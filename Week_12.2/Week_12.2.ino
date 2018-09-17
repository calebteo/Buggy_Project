#define IR_PIN 2        // setting digital pin 2 = IR_PIN
#define CTRL_PIN 3      // setting digital pin 3 = CTRL_PIN
#define GND A2          // UltraSonic
#define POW A3          // UltraSonic
#define SIGNAL A4       // UltraSonic

const int speedofsound = 34320;  //UltraSonic
unsigned long previousMillis = 0; //UltraSonic
const long interval = 1000; //Ultra Sonics

String incoming_string = ""; // string to store incoming data
bool string_complete = false;
volatile boolean ir_interrupt = false;   //  volatile qualifier as two seperate pieces of code need access
char char_array[2];

String buggy_id = "2";

void setup() {
  pinMode(IR_PIN, INPUT);
  pinMode(CTRL_PIN, OUTPUT);
  digitalWrite(CTRL_PIN, LOW);
  pinMode(IR_PIN, LOW);
  pinMode(GND, OUTPUT);
  pinMode(POW, OUTPUT);
  digitalWrite(GND, LOW);
  digitalWrite(POW, HIGH);
  pinMode(A4, OUTPUT);
  attachInterrupt(digitalPinToInterrupt(2), irISR, RISING);

  Serial.begin(9600);
  Serial.print("+++");
  delay(1500);
  Serial.println("ATID 1234, CH C, CN");
  delay(1500);
  //Serial.println ("Finished Configurating");

  while (Serial.read() != -1) {};
}

void loop() {
  if (string_complete) {
    //Serial.print("FLAG: " + incoming_string);
    if (incoming_string.substring(0, 1) == "2") {
      //Serial.print("FLAGED"); 
      buggyControl(incoming_string.substring(1, 2));
      incoming_string = "";
      string_complete = false;

    }
    else {
      incoming_string = "";
      string_complete = false;
    }
  }



  if (ir_interrupt) {
    gantryControl();
  }
  unsigned long currentMillis = millis();

  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;

//    while (obstacleDetection()) {
//      move(2);
//      if (!obstacleDetection()) {
//        move(4);
//      }
//    }
  }


}


// ************* functions **************
//  function for handling serial events
//  takes incoming characters, appends each to the last and generates
//  a string of characters. then sets string complete to true.
void serialEvent() {

  while (Serial.available() == 0) {
    /*do nothing*/
  }
  while (Serial.available()) {

    char incoming_char = (char)Serial.read();
    incoming_string += incoming_char;
    if (incoming_char == '\n') {
      string_complete = true;
    }
  }
}
// pulsewidth generation method
void move(int x) {
  delayMicroseconds(20000);
  digitalWrite(CTRL_PIN, HIGH);
  delayMicroseconds(x * 1000);
  digitalWrite(CTRL_PIN, LOW);
  delayMicroseconds(10);
}
// method for printing string to serial followed by newline
// flush method waits for transmission of outgoing serial data to complete
void writeXbee(String message) {
  Serial.println(message);
  Serial.flush();
  message = "";

}
// buggy control method
// dependent upon string incoming_string variable contents
// modified for just buggy 1
void buggyControl(String b) {

  // initial received command to establish communication

  if (b == "d") {
    writeXbee(String(buggy_id) + "D");
    incoming_string = "";
    string_complete = false;
  }
  // go control
  else if (b == "m") {
    move(4);
    writeXbee(String(buggy_id) + "M");
    incoming_string = "";
    string_complete = false;
  }
  // stop control
  else if (b == "s") {
    move(2);
    writeXbee(String(buggy_id) + "S");
    incoming_string = "";
    string_complete = false;

  }
  else if (b == "a") {
    writeXbee(String(buggy_id) + "A");
    incoming_string = "";
    string_complete = false;
  }
  else if (b == "b") {
    writeXbee(String(buggy_id) + "B");
    incoming_string = "";
    string_complete = false;
  }
  else if (b == "c") {
    writeXbee(String(buggy_id) + "C");
    incoming_string = "";
    string_complete = false;
  }


  else if (b == "l") {
    incoming_string = "";
    string_complete = false;
    move(8);
    delay(2000);
    move(4);        //Return to Normal
    delay(5000);     // wait until on straight
    move(2);        // Stop over line
    writeXbee(String(buggy_id) + "L");
  }

  else if (b == "r") {
    incoming_string = "";
    string_complete = false;
    move(6);
    delay(2000);
    move(4);        //Return to Normal
    delay(5000);     // wait until on straight
    move(2);        // Stop over line
    writeXbee(String(buggy_id) + "R");
  }

}
// read gantry method, invoked upon IR intterupt
int readGantry() {
  while (digitalRead(IR_PIN) != LOW);
  int pulse = pulseIn(IR_PIN, HIGH);
  // writeXbee (String(pulse));

  if (pulse >= 500 && pulse <= 1500) {   //500-1500
    return 1;
  }
  else if (pulse > 1500 && pulse <= 2500) {   //1500-2500
    return 2;
  }
  else if (pulse > 2500 && pulse <= 3500) {   //2500-3500
    return 3;
  }
  else {
    return -1;
  }
}
// Method to set IR interrupt true
void irISR () {
  ir_interrupt = true;
}
// Arduino gantry reached issue response to c# method
void gantryControl() {
  delay(500);
  move(2);
  int gantry = readGantry();
  if (gantry != -1) {
    Serial.println(String(buggy_id) + String(gantry));
  }

  ir_interrupt = false;
}

bool obstacleDetection() {
  pinMode(A4, OUTPUT);
  digitalWrite(A4, LOW);
  delayMicroseconds(2);
  digitalWrite(A4, HIGH);
  delayMicroseconds(10);
  digitalWrite(A4, LOW);
  pinMode(A4, INPUT);
  double echo_t = pulseIn(A4, HIGH);

  double distance = abs(((echo_t / 1000000) / 2) * speedofsound); //units in cm

  if (distance < 10)
    return true;
  else
    return false;

}





