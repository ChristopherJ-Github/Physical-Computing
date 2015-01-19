int out = 13;
int led = 12;
int button = 7;
int presses;
int delays[3] = {250, 500, 1000};

void setup() {                

  pinMode(led, OUTPUT);
  pinMode(out, OUTPUT); 
  digitalWrite(out, HIGH);
  pinMode(button, INPUT);
  
  Serial.begin(9600);    
}

void loop() {
  
  int buttonState = digitalRead(button);
  presses += buttonState;
  int _delay = getDelay(presses);
  
  digitalWrite(led, LOW);   
  delay(_delay);               
  digitalWrite(led, HIGH);   
  delay(_delay);  
}

int getDelay (int _presses) { //cycle through delays based on button press
  
  int index = _presses % sizeof(delays)/sizeof(delays[0]);
  int _delay = delays[index];
  return _delay;
}
