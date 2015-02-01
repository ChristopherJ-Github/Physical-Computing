import processing.serial.*;
import cc.arduino.*;
Arduino arduino;

int largestVal = 0; 
int smallestVal = 1023;
int calibrationCounter = 300;
int randomizationCounter = 150;
int buttonDelay = 13;
int buttonCounter = buttonDelay;
int buttonNumber = 0;
int numbers = 3;
float offsetAngle;
float correctAngle;
int correctAlpha;
int correctNumber;
PImage lock, lockBorder, lockMiddle, background, blueScreen;
PFont font;

int[] knobReadings;
int knobIndex = 0;
int knobTotal = 0;
int knobAverage = 0;

int[] lightReadings;
int lightIndex = 0;
int lightTotal = 0;
int lightAverage = 0;

String state = "setting";

//A program that turns on the lights in an image when physical lights are on

void setup() {
  
  size(1366, 768);
  println(Arduino.list());
  arduino = new Arduino(this, Arduino.list()[0], 57600);
  lock = loadImage("lock.png");
  lockBorder = loadImage("lockBorder.png");
  lockMiddle = loadImage("lockMiddle.png");
  background = loadImage("background.png");
  blueScreen = loadImage("blueScreen.png");
  font = loadFont("SimplifiedArabicFixed-100.vlw");
  lightReadings = new int[10];
  for (int i = 0; i < lightReadings.length; i++)
    lightReadings[i] = 0;
    
  knobReadings = new int[20];
  for (int i = 0; i < knobReadings.length; i++)
    knobReadings[i] = 0;
}

void draw() {
  
  int knob = arduino.analogRead(0);
  int smoothedKnob = smoothKnob(knob);
  int light = arduino.analogRead(1);
  int smoothedLight = smoothLight(light);
  int button = arduino.analogRead(2);
  int alpha = (int)map(smoothedLight, smallestVal, largestVal, 255, 0);
  alpha = constrain(alpha, 0, 255);
  float angle = map(smoothedKnob, 20, 1000, 2*PI, 0);
  
  if (state == "setting") {
    
    calibrate(light);
    updateShade(alpha);
    updateAngle(angle);
    updateButtonNumber (button);
    
    if (keyPressed) {
      if (key == 's' || key == 'S') {
        correctAngle = angle;
        correctAlpha = alpha;
        correctNumber = buttonNumber;
        state = "randomize";
      }
    }
  }
  
  if (state == "randomize") {
    
    updateShade(alpha);
    updateAngle(angle);
    updateButtonNumber (button);
    buttonNumber = (int)random(0, numbers);
    offsetAngle = (offsetAngle + (PI/20)) % (2 * PI);
    randomizationCounter --;
    if (randomizationCounter <= 0) 
      state = "locked";
  }
  
  if (state == "locked") {
    
    updateShade(alpha);
    updateAngle(angle);
    updateButtonNumber (button);
    boolean correct = checkIfCorrect(alpha, angle);
    
    if (correct) {
      state = "unlocked";
    }
  }
  
  if (state == "unlocked") {
    image(blueScreen, 0, 0, width, height);
  }
}

int smoothLight (int light) {
  
  lightTotal = lightTotal - lightReadings[lightIndex];
  lightReadings[lightIndex] = light;
  lightTotal += lightReadings[lightIndex];
  lightIndex = (lightIndex + 1) % lightReadings.length;
  return lightTotal/lightReadings.length;
}

int smoothKnob (int knob) {
  
  knobTotal = knobTotal - knobReadings[knobIndex];
  knobReadings[knobIndex] = knob;
  knobTotal += knobReadings[knobIndex];
  knobIndex = (knobIndex + 1) % knobReadings.length;
  return knobTotal/knobReadings.length;
}

void calibrate (int light) {
  
  if (light == 0)
    return;
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

void updateShade (int alpha) {
  
  fill(alpha, alpha, alpha);
  rect(0, 0, width, height);
  image(background, 0, 0, width, height);
}

void updateAngle (float angle) {
  
  angle = (angle + offsetAngle) % (2 * PI); //for randomizing after setting
  
  pushMatrix ();
  translate(width/2, height/2);
  rotate(angle);
  translate(-width/2, -height/2);
  image(lock, width/2 - 200, height/2 -200, 400, 400);
  popMatrix ();
  image(lockBorder, width/2 - 200, height/2 - 200, 400, 400);
}

void updateButtonNumber (int button) {
  
  buttonCounter --;
  if (buttonCounter <= 0) {
    if (button > 0) {
      buttonCounter = buttonDelay;
      buttonNumber = (buttonNumber + 1) % numbers;
    }
  }
  fill(0,0,0,200);
  textAlign(CENTER, CENTER);
  textFont(font, 100);
  text(buttonNumber + 1, width/2, height/2);
  image(lockMiddle, width/2 - 200, height/2 - 200, 400, 400);
}

boolean checkIfCorrect (int alpha, float angle) {
  
  float threshold = 1.05;
  boolean angleCorrect = false;
  boolean alphaCorrect = false;
  boolean buttonCorrect = false;
  
  float twoPI = 2 * PI;
  float offsetedCorrectAngle = (((correctAngle - offsetAngle) % twoPI) + twoPI) % twoPI;
  if (angle <= offsetedCorrectAngle * threshold && angle >= offsetedCorrectAngle / threshold) 
    angleCorrect = true;
  
  threshold = 1.7;
  if (alpha <= correctAlpha * threshold && alpha >= correctAlpha / threshold) 
    alphaCorrect = true;
  
  if (buttonNumber == correctNumber) 
    buttonCorrect = true;
  
  if (angleCorrect && alphaCorrect && buttonCorrect) {
   return true;
  } else {
   return false;
  }
 
}
