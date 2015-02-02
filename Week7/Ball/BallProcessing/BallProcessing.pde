import processing.serial.*;

float boxX;
float boxY;
int ballSize = 56;
PVector position; 
float i;
String state = "gettingInput";

Serial port;

//A program that pushes a ball off a box with a servo when the screen is clicked

void setup() {
  size(500, 500);
  println(Serial.list());
  port = new Serial(this, Serial.list()[0], 9600);
  port.write('L'); //reset rotation on servo
  position = new PVector (width/2, height/2);
}

void draw() {
  
  if (state == "gettingInput") {
     
    if (mousePressed && (mouseButton == LEFT)) {
      port.write('H'); //rotate servo
      state = "pushed";
    }
  }
  
  if (state == "pushed") {
    
    i += 0.05;
    position.x -= i * 1.3;
    position.y += i * i;
  }
  
  background(0);
  fill(255,255,255);
  ellipse(position.x - (ballSize/9), position.y - (ballSize/2), ballSize, ballSize);
  rect(width/2, height/2, 300, 300);
}

