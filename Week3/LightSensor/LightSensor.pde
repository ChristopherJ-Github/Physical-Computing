import processing.serial.*;
import cc.arduino.*;
Arduino arduino;

float largestVal = 0; 
float smallestVal = 1023;
float calibrationCounter = 300;
PImage image;

int[] readings;
int index = 0;
int total = 0;
int average = 0;

//A program that turns on the lights in an image when physical lights are on

void setup() {
  
  size(720, 480);
  println(Arduino.list());
  arduino = new Arduino(this, Arduino.list()[0], 57600);
  image = loadImage("2ch.jpg");
  readings = new int[10];
  for (int i = 0; i < readings.length; i++)
    readings[i] = 0;
}

void draw() {
  
  int light = arduino.analogRead(0);
  int smoothedLight = smoothLight(light);
  calibrate(light);
  float alpha = map(smoothedLight, smallestVal, largestVal, 255, 0);
  drawImage(alpha);
}

int smoothLight (int light) {
  
  total = total - readings[index];
  readings[index] = light;
  total += readings[index];
  index = (index + 1) % readings.length;
  return total/readings.length;
}

void calibrate (float light) {
  
  calibrationCounter --;
  if (calibrationCounter == 0)
    println("Done Calibration");
  if (calibrationCounter <= 0)
    return;
    
  if (light > largestVal) { 
    largestVal = light;
  }
  if (light < smallestVal) {
    smallestVal = light;
  } 
}

void drawImage (float alpha) {
  
  image(image, 0, 0, width, height);
  fill(0, 0, 0, alpha);
  rect(0, 0, width, height);
}
    


