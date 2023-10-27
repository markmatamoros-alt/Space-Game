/*****************************************************************************
* Testing script for Space Game physical controls, utilizing and Arduino Uno
* Includes printing to serial display for debugging
* 
* All digital inputs are set with the internal Pull-up resistors
*   -logic: physical activation will read "0," no activation will read "1"
* 
* Ensure that the "LiquidCrystal_I2C" and "FastLED" libraries are downloaded
*   -utilize library manager if necessary
* 
* LCD initialization and handling occurs within setup
*   -should display "left LCD" and "right LCD" respectively
*   -right LCD has bridging on the A0 pins for addressing purposes  
*   
* LEDs are addressed with the following:
*   Pot LED 1: 0
*   Pot LED 2: 2
*   Pot LED 3: 4
*   Pot LED 4: 6
*   Pot LED 5: 8
*   Button LED 1: 40
*   Button LED 2: 42
*   Button LED 3: 44
*   
*   -These addresses are utilized for testing purposes
*   -These values can be adjusted with the potLED# and buttLED# variables
*   
* Pot mapping has been set to handle 3.3v
* 
* Refer to Circuit Diagram for pin numbers
* 
* Contact me if you have an questions/difficulties: mark@escapetheroomnyc.com
**********************************************************************************/
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <FastLED.h>

#define UPDATES_PER_SECOND 100
LiquidCrystal_I2C lcd_leftSide(0x27,16,2);
LiquidCrystal_I2C lcd_rightSide(0x26,16,2);

#define LED_PIN     9     //pin# for data
#define NUM_LEDS    50
#define BRIGHTNESS  200
#define LED_TYPE    WS2811
#define COLOR_ORDER GRB
CRGB leds[NUM_LEDS];

//ADJUSTABLE LED ADDRESSES
int potLED1 = 0;
int potLED2 = 2;
int potLED3 = 4;
int potLED4 = 6;
int potLED5 = 8;

int buttLED1 = 40;
int buttLED2 = 42;
int buttLED3 = 44;

//text for LCD displays
String lcd_leftSide_Title = "Left LCD";
String lcd_rightSide_Title = "Right LCD";

//pin numbers for physical controls
int rockerInputPins[4] = {2,3,4,5};
int buttonInputPins[3] = {6,7,8};
int joyStickPins[4] = {10,11,12,13};

int potValue = 0;   //hold recieved potentiometer value for evaluation/mapping
int potLEDNum = 0;  //holds LED address for LED activation

void setup() {
  Serial.begin(9600);

  //set control pins 
  for (int i = 2; i < 14; i++)
  {
    if (i != 9)
    {
      pinMode(i, INPUT_PULLUP);
    }
  }

  //potentiometer Pin
  pinMode(A0, INPUT);

  //handles LCD initialization and pushes text
  initializeAndTestLCDs();

  //prep LED strip handling
  FastLED.addLeds<LED_TYPE, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.setBrightness(  BRIGHTNESS );
}

void loop() 
{
  determinePotLED();    //handles LED selection for pot
  activateLEDs();       //handles LED addressing with color/saturation values

  monitorInputs();      //utilize serial monitor for debugging

  //push data to LED strip
  FastLED.show();
  FastLED.delay(500 / UPDATES_PER_SECOND);
}

/**************************************************
* Initializes LCDs and populates screen with text
***************************************************/
void initializeAndTestLCDs()
{
  lcd_leftSide.init();      
  lcd_leftSide.backlight();  
  lcd_leftSide.clear();
  lcd_leftSide.setCursor(0,0);
  lcd_leftSide.print(lcd_leftSide_Title);
  
  lcd_rightSide.init();       
  lcd_rightSide.backlight();  
  lcd_rightSide.clear();
  lcd_rightSide.setCursor(0,0);
  lcd_rightSide.print(lcd_rightSide_Title); 
}

/**************************************************
* Map pot values and determine LED# for activation
**************************************************/
void determinePotLED()
{
  //map values and store
  potValue = map(analogRead(A0), 0, 695, 1, 6); 

    //determine LED for activation
    switch (potValue)
    {
      case 1:
        potLEDNum = potLED1;
        break;
      case 2:
        potLEDNum = potLED2;
        break;
      case 3:
        potLEDNum = potLED3;
        break;
      case 4:
        potLEDNum = potLED4;
        break;
      default:
        potLEDNum = potLED5;
        break;
    }
}

/*********************************************
* Sets LEDs for activation: pot and buttons
**********************************************/
void activateLEDs()
{
  //turn OFF all LEDs except active POT LED
  for (int i = 0; i < 20; i++)
  {
    if(i != potLEDNum)
    {
      leds[i] = ColorFromPalette(RainbowColors_p, 190, 0, LINEARBLEND);
    }
  }

  //handles LED for button 1
  if (digitalRead(buttonInputPins[0]) == LOW)
  {
     leds[buttLED1] = ColorFromPalette(RainbowColors_p, 190, 250, LINEARBLEND);
  }
  else
  {
     leds[buttLED1] = ColorFromPalette(RainbowColors_p, 190, 0, LINEARBLEND);
  }

  //handles LED for button 2
  if (digitalRead(buttonInputPins[1]) == LOW)
  {
     leds[buttLED2] = ColorFromPalette(RainbowColors_p, 190, 250, LINEARBLEND);
  }  
  else
  {
     leds[buttLED2] = ColorFromPalette(RainbowColors_p, 190, 0, LINEARBLEND);
  }  

  //handles LED for button 3
  if (digitalRead(buttonInputPins[2]) == LOW)
  {
     leds[buttLED3] = ColorFromPalette(RainbowColors_p, 190, 250, LINEARBLEND);
  }
  else
  {
     leds[buttLED3] = ColorFromPalette(RainbowColors_p, 190, 1, LINEARBLEND);
  }

  //set Pot LED
  leds[potLEDNum] = ColorFromPalette(RainbowColors_p, 190, 250, LINEARBLEND);
}

/************************************************
* Prints values to serial monitor for debugging
************************************************/
void monitorInputs()
{
  //display rocker values
  for (int i = 2; i < 6; i++) 
  {
    Serial.print("Rock " + String(i - 1) + ": " + String(digitalRead(i)) + " ");
  }

  //display button values
  for (int i = 6; i < 9; i++)
  {
    Serial.print("Butt " + String(i - 5) + ": " + String(digitalRead(i)) + " ");
  }

  //display joystick direction values
  for (int i = 10; i < 14; i++)
  {
    Serial.print("Stick Dir " + String(i - 9) + ": " + String(digitalRead(i)) + " ");
  }

  //display potentiometer value
  Serial.print("Pot: "+ String(analogRead(A0)));
  Serial.println("");
}
