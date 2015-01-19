int out = 13;
int led = 12;
int buttons[3] = {7, 6, 5};
int delays[3] = {250, 500, 1000};

void setup() {                

  pinMode(led, OUTPUT);
  pinMode(out, OUTPUT); 
  digitalWrite(out, HIGH);
  
  for (int i = 0; i < sizeof(buttons)/sizeof(buttons[0]); i++) 
    pinMode(buttons[i], INPUT);
  
  Serial.begin(9600);    
}

void loop() {
  
  int buttonStates[sizeof(buttons)/sizeof(buttons[0])];
  for (int i = 0; i < sizeof(buttons)/sizeof(buttons[0]) + 1; i++)
    buttonStates[i] = digitalRead(buttons[i]);
    
  int _delay = getDelay(buttonStates);
  digitalWrite(led, LOW);   
  delay(_delay);               
  digitalWrite(led, HIGH);   
  delay(_delay);  
  Serial.println(_delay);
}

int getDelay (int buttonStates[]) { //if one button is pressed it's corresponding delay is return 
  
  int _delay, amountPressed;
  for (int i = 0; i < sizeof(buttonStates)/sizeof(buttonStates[0]); i++) {
    if (buttonStates[i]) {
      _delay = delays[i];
      amountPressed ++;
    } 
  }
  if (amountPressed > 1 || amountPressed == 0) 
    return 0;
  else
    return _delay;
}
