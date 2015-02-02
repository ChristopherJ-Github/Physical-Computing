import processing.serial.*;

Serial myPort;
Animation animation;
int frames = 19;

float largestVal = 0; 
float smallestVal = 1023;

float[] readings;
int index = 0;
float total = 0;
float average = 0;

//A program that blooms a flower when a light sensor is given light

void setup() {
  
  size(500, 281);
  frameRate(24);
  animation = new Animation("frame_", frames);
  
  println(Serial.list());
  myPort = new Serial(this, Serial.list()[0], 9600);
  myPort.bufferUntil('\n');
  
  readings = new float[3];
  for (int i = 0; i < readings.length; i++)
    readings[i] = 0;
}

void draw() { 
  
}

void serialEvent (Serial myPort) {
  
  String inString = new String(myPort.readBytesUntil('\n'));
  
  if (inString != null) {
    
    float light = float(inString);
    if(!Float.isNaN(light)) {
      
      calibrate(light);
      float smoothedLight = smoothLight(light);
      int frame = (int)map(smoothedLight, largestVal, smallestVal, 0, frames - 1);
      frame = (int)constrain(frame, 0, frames -1);
      animation.display(frame);
    }
  }
}

// Class for animating a sequence of GIFs
class Animation {
  PImage[] images;
  int imageCount;
  int frame;
  
  Animation(String imagePrefix, int count) {
    
    imageCount = count;
    images = new PImage[imageCount];

    for (int i = 0; i < imageCount; i++) {
      String filename = imagePrefix + nf(i, 3) + ".gif";
      images[i] = loadImage(filename);
    }
  }

  void display(int frame) {
    
    image(images[frame], 0, 0);
  }
}

float smoothLight (float light) {
  
  total = total - readings[index];
  readings[index] = light;
  total += readings[index];
  index = (index + 1) % readings.length;
  return total/readings.length;
}

void calibrate (float light) {
    
  if (light > largestVal) { 
    largestVal = light;
  }
  if (light < smallestVal) {
    smallestVal = light;
  } 
}
