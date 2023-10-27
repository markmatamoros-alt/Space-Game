//#include <ByteConvert.hpp>

#include <Servo.h>
#define MAXSERVOS 8

#include<Uduino.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <FastLED.h>

Uduino uduino("spaceBoard");

//********************LCDs***********************/
LiquidCrystal_I2C lcd_leftSide(0x27, 16, 2);
LiquidCrystal_I2C lcd_rightSide(0x26, 16, 2);

String leftText = "";
String rightText = "";

/********************LEDs************************/
//LED testing is included within LCD functions
#define LED_PIN     9
#define NUM_LEDS    50 
//50
#define BRIGHTNESS  200
#define LED_TYPE    WS2811
//#define LED_TYPE    WS2812B
#define COLOR_ORDER GRB
CRGB leds[NUM_LEDS];

int tempLEDTester = 1;

#define UPDATES_PER_SECOND 100
/************************************************/

void setup() {
  Serial.begin(9600);

  ////////////////Copied from the example script "Uduino"///////////////
#if defined (__AVR_ATmega32U4__) // Leonardo
  while (!Serial) {}
#elif defined(__PIC32MX__)
  delay(1000);
#endif

  uduino.addCommand("s", SetMode);
  uduino.addCommand("d", WritePinDigital);
  uduino.addCommand("a", WritePinAnalog);
  uduino.addCommand("rd", ReadDigitalPin);
  uduino.addCommand("r", ReadAnalogPin);
  uduino.addCommand("br", BundleReadPin);
  uduino.addCommand("b", ReadBundle);
  uduino.addInitFunction(DisconnectAllServos);
  uduino.addDisconnectedFunction(DisconnectAllServos);
  /////////////////End Copied Part/////////////////////////////////////

  //LCD
  uduino.addCommand("pL_LCD", PopulateLeftLCD);
  uduino.addCommand("pR_LCD", PopulateRightLCD);
  uduino.addCommand("c_LCDs", ClearLCDs);

  //LED
  uduino.addCommand("setLED", SetLed);
  uduino.addInitFunction(InitLcdLedAndInputs);

}

void loop()
{
  uduino.update();

  FastLED.show();
  FastLED.delay(500 / UPDATES_PER_SECOND);


}

/********************************************************
   Handles Left LCD Populating: Recieves text from Unity
   Includes increasing LED Intensity
 ********************************************************/
void PopulateLeftLCD()
{
  //int byteArraySize= int(uduino.getParameter(1));
  //byte byteArray=byte(uduino.getParameter(0));
  //leftText = ByteConvert::arrayToString(byteArraySize,byte(uduino.getParameter(0)));
  //byte byteArray[int(uduino.getParameter(1))];
  //leftText = String(byteArray);
  
 leftText = String(uduino.getParameter(0));
  String fullText = String(leftText);
  String textFirstHalf = "";
  String textSecondHalf = "";
  Serial.println( leftText);
  fullText.replace("_", " ");

  lcd_leftSide.clear();

  if (fullText.length() >= 16)
  {
    if (fullText.charAt(16) != ' ')
    {      
      for (int i = 16; i > 0; i--)
      {        
        if (fullText.charAt(i) == ' ')
        {
          textFirstHalf = fullText.substring(0, i);
          textSecondHalf = fullText.substring(i+1, fullText.length());
          break;
        }
      }
    } else {
      textFirstHalf = fullText.substring(0, 16);
      textSecondHalf = fullText.substring(17, fullText.length());
    }
    lcd_leftSide.setCursor(0, 0);
    lcd_leftSide.print(textFirstHalf);

    lcd_leftSide.setCursor(0, 1);
    lcd_leftSide.print(textSecondHalf);

  } else {
    lcd_leftSide.setCursor(0, 0);
    lcd_leftSide.print(fullText);
  }


  //leds[40] = ColorFromPalette(RainbowColors_p, 190, 250, LINEARBLEND);
  //leds[41] = ColorFromPalette(RainbowColors_p, 190, 250, LINEARBLEND);
  //leds[49] = ColorFromPalette(RainbowColors_p, 190, 250, LINEARBLEND);
}

/********************************************************
   Handles Right LCD Populating: Recieves text from Unity
   Includes lowering LED Inensity
 ********************************************************/
void PopulateRightLCD()
{
 rightText = String(uduino.getParameter(0));
  String fullText = rightText;
  String textFirstHalf = "";
  String textSecondHalf = "";
  Serial.println( rightText);
  fullText.replace("_", " ");

  lcd_rightSide.clear();

  if (fullText.length() >= 16)
  {
    if (fullText.charAt(16) != ' ')
    {      
      for (int i = 16; i > 0; i--)
      {        
        if (fullText.charAt(i) == ' ')
        {
          textFirstHalf = fullText.substring(0, i);
          textSecondHalf = fullText.substring(i+1, fullText.length());
          break;
        }
      }
    } else {
      textFirstHalf = fullText.substring(0, 16);
      textSecondHalf = fullText.substring(17, fullText.length());
    }

    lcd_rightSide.setCursor(0, 0);
    lcd_rightSide.print(textFirstHalf);

    lcd_rightSide.setCursor(0, 1);
    lcd_rightSide.print(textSecondHalf);

  } else {
    lcd_rightSide.setCursor(0, 0);
    lcd_rightSide.print(fullText);
  }


  //leds[40] = ColorFromPalette(RainbowColors_p, 190, 10, LINEARBLEND);
  //leds[41] = ColorFromPalette(RainbowColors_p, 190, 10, LINEARBLEND);
  //leds[49] = ColorFromPalette(RainbowColors_p, 190, 10, LINEARBLEND);
}

/********************************************************
   Handles LCD Clearing: Recieves command from Unity
 ********************************************************/
void ClearLCDs()
{
  lcd_leftSide.clear();
  lcd_rightSide.clear();
  leftText = "";
  rightText = "";
}
/********************************************************
   Set LED: Receives command from Unity
 ********************************************************/
void SetLed()
{
  int led = atoi(uduino.getParameter(0));
  int r = atoi(uduino.getParameter(1));
  int g = atoi(uduino.getParameter(2));
  int b = atoi(uduino.getParameter(3));
  leds[led].setRGB(r, g, b);
  FastLED.show();

}

void SetLedRGB(int _led, int _r, int _g, int _b)
{
  leds[_led].setRGB(_r, _g, _b);
    //leds[_led] = CRGB(_r, _g, _b);

  FastLED.show();

}

/**************************************************************
   Handles LCD, LED, and hardware initialization
   Also calls funciton for Arduino IO initialization
 **************************************************************/
void InitLcdLedAndInputs()
{
  lcd_leftSide.init();       //initialize lcd_leftSide
  lcd_leftSide.backlight();  //turn on back light
  lcd_leftSide.clear();

  lcd_rightSide.init();       //initialize lcd_leftSide
  lcd_rightSide.backlight();  //turn on back light
  lcd_rightSide.clear();
  //FastLED.addLeds<WS2812, LED_PIN, GRB>(leds, NUM_LEDS);

  FastLED.addLeds<LED_TYPE, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.setBrightness(  BRIGHTNESS );
}

////////////////Copied from the example script "Uduino"///////////////
void ReadBundle() {
  char *arg = NULL;
  char *number = NULL;
  number = uduino.next();
  int len = 0;
  if (number != NULL)
    len = atoi(number);
  for (int i = 0; i < len; i++) {
    uduino.launchCommand(arg);
  }
}

void SetMode() {
  int pinToMap = 100; //100 is never reached
  char *arg = NULL;
  arg = uduino.next();
  if (arg != NULL)
  {
    pinToMap = atoi(arg);
  }
  int type;
  arg = uduino.next();
  if (arg != NULL)
  {
    type = atoi(arg);
    PinSetMode(pinToMap, type);
  }
}

void PinSetMode(int pin, int type) {
  //TODO : vérifier que ça, ça fonctionne
  if (type != 4)
    DisconnectServo(pin);

  switch (type) {
    case 0: // Output
      pinMode(pin, OUTPUT);
      break;
    case 1: // PWM
      pinMode(pin, OUTPUT);
      break;
    case 2: // Analog
      pinMode(pin, INPUT);
      break;
    case 3: // Input_Pullup
      pinMode(pin, INPUT_PULLUP);
      break;
    case 4: // Servo
      SetupServo(pin);
      break;
  }
}

void WritePinAnalog() {
  int pinToMap = 100;
  char *arg = NULL;
  arg = uduino.next();
  if (arg != NULL)
  {
    pinToMap = atoi(arg);
  }

  int valueToWrite;
  arg = uduino.next();
  if (arg != NULL)
  {
    valueToWrite = atoi(arg);

    if (ServoConnectedPin(pinToMap)) {
      UpdateServo(pinToMap, valueToWrite);
    } else {
      analogWrite(pinToMap, valueToWrite);
    }
  }
}

void WritePinDigital() {
  int pinToMap = -1;
  char *arg = NULL;
  arg = uduino.next();
  if (arg != NULL)
    pinToMap = atoi(arg);

  int writeValue;
  arg = uduino.next();
  if (arg != NULL && pinToMap != -1)
  {
    writeValue = atoi(arg);
    digitalWrite(pinToMap, writeValue);
  }
}

void ReadAnalogPin() {
  int pinToRead = -1;
  char *arg = NULL;
  arg = uduino.next();
  if (arg != NULL)
  {
    pinToRead = atoi(arg);
    if (pinToRead != -1)
      printValue(pinToRead, analogRead(pinToRead));
  }
}

void ReadDigitalPin() {
  int pinToRead = -1;
  char *arg = NULL;
  arg = uduino.next();
  if (arg != NULL)
  {
    pinToRead = atoi(arg);
  }

  if (pinToRead != -1)
    printValue(pinToRead, digitalRead(pinToRead));
}

void BundleReadPin() {
  int pinToRead = -1;
  char *arg = NULL;
  arg = uduino.next();
  if (arg != NULL)
  {
    pinToRead = atoi(arg);
    if (pinToRead != -1) {
      if (pinToRead < 13)
        printValue(pinToRead, digitalRead(pinToRead));
      else
        printValue(pinToRead, analogRead(pinToRead));
    }
  }
}

Servo myservo;
/*void loop()
  {
  uduino.update();
  }*/

void printValue(int pin, int targetValue) {
  uduino.print(pin);
  uduino.print(" "); //<- Todo : Change that with Uduino delimiter
  uduino.println(targetValue);
  // TODO : Here we could put bundle read multiple pins if(Bundle) { ... add delimiter // } ...
}




/* SERVO CODE */
Servo servos[MAXSERVOS];
int servoPinMap[MAXSERVOS];
/*
  void InitializeServos() {
  for (int i = 0; i < MAXSERVOS - 1; i++ ) {
    servoPinMap[i] = -1;
    servos[i].detach();
  }
  }
*/
void SetupServo(int pin) {
  if (ServoConnectedPin(pin))
    return;

  int nextIndex = GetAvailableIndexByPin(-1);
  if (nextIndex == -1)
    nextIndex = 0;
  servos[nextIndex].attach(pin);
  servoPinMap[nextIndex] = pin;
}


void DisconnectServo(int pin) {
  servos[GetAvailableIndexByPin(pin)].detach();
  servoPinMap[GetAvailableIndexByPin(pin)] = 0;
}

bool ServoConnectedPin(int pin) {
  if (GetAvailableIndexByPin(pin) == -1) return false;
  else return true;
}

int GetAvailableIndexByPin(int pin) {
  for (int i = 0; i < MAXSERVOS - 1; i++ ) {
    if (servoPinMap[i] == pin) {
      return i;
    } else if (pin == -1 && servoPinMap[i] < 0) {
      return i; // return the first available index
    }
  }
  return -1;
}

void UpdateServo(int pin, int targetValue) {
  int index = GetAvailableIndexByPin(pin);
  servos[index].write(targetValue);
  delay(10);
}

void DisconnectAllServos() {
  for (int i = 0; i < MAXSERVOS; i++) {
    servos[i].detach();
    digitalWrite(servoPinMap[i], LOW);
    servoPinMap[i] = -1;
  }
}
/////////////////End Copied Part/////////////////////////////////////
