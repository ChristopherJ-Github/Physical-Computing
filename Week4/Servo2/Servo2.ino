#include <Servo.h> 
 
Servo myservo;
int pos = 0;   
 
void setup() { 
  
  myservo.attach(9);  
} 
 
void loop() {      
  
  if (pos == 0) {
    pos = 180;
  } else {
    pos = 0;
  }
  delay (15);    
} 
