#include <Servo.h> 
 
Servo myservo;
int pos = 0;   
 
void setup() { 
  
  myservo.attach(9);  
} 
 
void loop() {      
  
  pos = (pos + 1) % 180;
  myservo.write(pos); 
  
  delay (15);    
} 
