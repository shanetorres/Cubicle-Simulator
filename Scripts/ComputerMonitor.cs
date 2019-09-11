// - ComputerMonitor.cs
// Functions for the world space computer monitor canvas, such as typing, as well as the sentence rearrangement algorithm (in progress)
// Code written by Shane Torres.

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngineInternal;

public class ComputerMonitor : MonoBehaviour
{
    private static bool s_MonitorFocused;
    GameObject Text;
    GameObject scanvas;
    private float m_TimeStamp;
    private bool cursor = false;
    private string cursorChar;
    private int cursorIndex;
    private bool cursorAtEnd;
    private int lineStart;
    private bool startedWork;
    private int randomSentenceNumber;
    private int totalUsedSentences;
    private List<string> Sentences = new List<string>(new string[] {"This crazy sentence is good", "This sentence is crazy good",
        "I went to go lake water swimming in the park", "I went to the park to go swimming in lake water",
        "She visited her grandmother on Sunday", "On Sunday she visited her grandmother",
        "Buffalo buffalo buffalo Buffalo buffalo buffalo", "Buffalo buffalo buffalo Buffalo buffalo buffalo", });


    // Start is called before the first frame update
    void Start()
    {
        startedWork = false;
        Text = GameObject.FindGameObjectWithTag("text");
        scanvas = GameObject.FindGameObjectWithTag("screencanvas");
        Text.GetComponent<TMPro.TextMeshProUGUI>().text = "Enter Username: user1982\nEnter Password:\nWelcome back user1982! Are you ready to begin work again? (y/n)\n> ";
        cursorAtEnd = true;
        cursorIndex = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Length;
        lineStart = cursorIndex;
        // Scroll screen to bottom at the start of the game.
        Canvas.ForceUpdateCanvases();
        scanvas.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLength();
        Cursor();
        MoveCursor();
        s_MonitorFocused = FirstPersonController.m_MonitorFocused;
        if (s_MonitorFocused)
        {
            // AUTOSCROLL SCREEN DOWN
            Canvas.ForceUpdateCanvases();
            scanvas.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
            WriteText();
        }
    }

    private void CheckLength()
    {
        if (Text.GetComponent<TMPro.TextMeshProUGUI>().text.Length > 2500)
        {
            string trimmedText = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Substring(500);
            lineStart -= 500;
            cursorIndex -= 500;
            Text.GetComponent<TMPro.TextMeshProUGUI>().text = trimmedText;
        }
    }

    void Cursor()
    {
            // BLINK THE CURSOR
            if (Time.time - m_TimeStamp >= 0.5)
            {
                m_TimeStamp = Time.time;
                if (cursor == false)
                {
                    cursor = true;
                    cursorChar = "|";
                    string cursorString = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Insert(cursorIndex, cursorChar);
                    Text.GetComponent<TMPro.TextMeshProUGUI>().text = cursorString;
                }
                else if (cursor == true)
                { 
                    cursor = false;
                    cursorChar = "";
                    string withoutCursor = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Replace("|", "");
                    Text.GetComponent<TMPro.TextMeshProUGUI>().text = withoutCursor;
                }
            }
            // CHECK IF THE CURSOR IS AT THE END OF THE LINE
            if (cursorIndex == Text.GetComponent<TMPro.TextMeshProUGUI>().text.Length)
            {
                cursorAtEnd = true;
            }
    }

    void MoveCursor()
    {
        // MOVE CURSOR LEFT IF IT IS NOT AT THE LINE START
        if (Input.GetKeyDown(KeyCode.LeftArrow) && (cursorIndex - lineStart > 0))
        {
            string previousText = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Replace("|","");
            Text.GetComponent<TMPro.TextMeshProUGUI>().text = previousText;
            cursorIndex--;
            cursorAtEnd = false;
            if (cursorChar == "|")
            {
                string cursorString = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Insert(cursorIndex, cursorChar);
                Text.GetComponent<TMPro.TextMeshProUGUI>().text = cursorString;
                cursor = true;
            }
        }
        // MOVE CURSOR RIGHT IF IT IS NOT AT THE LINE END
        if (Input.GetKeyDown(KeyCode.RightArrow) && !cursorAtEnd)
        {
            string previousText = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Replace("|", "");
            Text.GetComponent<TMPro.TextMeshProUGUI>().text = previousText;
            cursorIndex++;
            cursorAtEnd = false;
            if (cursorChar == "|")
            {
                string cursorString = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Insert(cursorIndex, cursorChar);
                Text.GetComponent<TMPro.TextMeshProUGUI>().text = cursorString;
                cursor = true;
            }
        }
    }

    void WriteText()
    {
        //todo: holding down backspace and arrow key functionality?
        // WRITE USER INPUT TO COMPUTER SCREEN
        string previousText = Text.GetComponent<TMPro.TextMeshProUGUI>().text;
        if (cursorChar == "|" && cursorAtEnd)
        {
            previousText = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Replace("|", "");
            cursorIndex = previousText.Length;
        }
        else
        {
            previousText = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Replace("|", "");
        }
        // DELETE THE CHARACTER BEFORE THE CURSOR
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (cursorIndex - lineStart > 0)
            {
                // when backspace is pressed, the end character from the string is removed
                string previous = Text.GetComponent<TMPro.TextMeshProUGUI>().text;
                string newstring = previous.Remove(cursorIndex - 1, 1);
                cursorIndex--;
                Text.GetComponent<TMPro.TextMeshProUGUI>().text = newstring;
            }
        }
        // START A NEW LINE WITH > 
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            string enteredSentence = previousText.Substring(lineStart-1);
            previousText = CheckSentence(previousText, enteredSentence);
            Text.GetComponent<TMPro.TextMeshProUGUI>().text = previousText + "\n> ";
            cursorIndex = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Length;
            lineStart = cursorIndex;
            if (cursorChar == "|")
            {
                string cursorString = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Insert(cursorIndex, cursorChar);
                Text.GetComponent<TMPro.TextMeshProUGUI>().text = cursorString;
                cursor = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("move");
        }
        // INSERT TYPED CHARACTERS INTO THE SCREEN TEXT
        else
        {
            // when any key other than backspace is pressed, it is written to the screen.
            foreach (char c in Input.inputString)
            {
                // maybe put the backspace functionality in here so that you can hold the button down?
                if (cursorAtEnd)
                {
                    Text.GetComponent<TMPro.TextMeshProUGUI>().text = previousText + c + cursorChar;
                    cursorIndex = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Length;
                }
                else
                { 
                    string newText = Text.GetComponent<TMPro.TextMeshProUGUI>().text.Insert(cursorIndex, c.ToString());
                    Text.GetComponent<TMPro.TextMeshProUGUI>().text = newText;
                    cursorIndex++;
                }
            }
        }
    }

    // CHECK THE USER SENTENCE AGAINST A PRIME EXAMPLE OF REARRANGEMENT.
    private string CheckSentence(string previousText, string enteredSentence)
    {
        // THE USER HAS NOT ENTERED WORK MODE YET.
        if (startedWork == false)
        {
            char lastCharacter = previousText[previousText.Length - 1];
            if (lastCharacter.ToString() == "y")
            {
                randomSentenceNumber = UnityEngine.Random.Range(0, Sentences.Count);
                if (randomSentenceNumber % 2 != 0)
                {
                    randomSentenceNumber--;
                }

                previousText += "\n\nWelcome back to the training of the Sentence Rearrangement Algorithm v0.8.2\n" +
                    "Please remain diligent in feeding the algorithm samples of rearranged sentences.\n" +
                    "Remember, the neural network isn't going to \"neur\"-ish itself!" +
                    "\n\nPlease rearrange the following sentence:\n"
                    + Sentences[randomSentenceNumber];
                startedWork = true;
            }
            else
            {

                previousText += "\nREMINDER FROM SYSTEM ADMINISTRATOR: You will not be paid for time that is not spent in working mode," +
                    " and you also may not leave until you have reached 8 hours in working mode.\n" +
                    "Current work mode period for Tuesday, July 21 is 7 hours, 49 minutes.\n Type (y) when you are ready to return to working mode.";
            }
        }
        //THE USER IS IN WORK MODE.n 
        else
        {
            // Sometimes an empty space is at the start of the string which
            // messes up the score, kind of hacky but this removes that space.
            if (enteredSentence[0] == ' ')
            {
                enteredSentence = enteredSentence.Substring(1);
            }

            string[] compareSentence = Sentences[randomSentenceNumber + 1].Split(' ');
            string[] userSentence = enteredSentence.Split(' ');

            // FILL ARRAY WITH SPACES IF IT IS NOT THE SAME LENGTH AS COMPARE SENTENCE.
            if (userSentence.Length < compareSentence.Length)
            {
                int differenceInLengths = compareSentence.Length - userSentence.Length;
                for (int i = 0; i < differenceInLengths; i++)
                {
                    Array.Resize<string>(ref userSentence, userSentence.Length + 1);
                    userSentence[userSentence.Length - 1] = "fill";
                }
            }

            float stringPoints = 0.0f;
            // COMPARE THE LIKENESS OF BOTH SENTENCES.
            for (int i = 0; i < compareSentence.Length; i++)
            {
                if (compareSentence[i].ToLower() == userSentence[i].ToLower())
                {
                    stringPoints += 1.0f;
                }
            }
            float score = stringPoints / compareSentence.Length;

            Sentences.RemoveAt(randomSentenceNumber);
            Sentences.RemoveAt(randomSentenceNumber);
            // IF ALL OF THE SENTENCES HAVE BEEN USED UP.
            if (Sentences.Count == 0)
            {
                ResetSentences();
            }

            // GET ANOTHER RANDOM SENTENCE.
            randomSentenceNumber = UnityEngine.Random.Range(0, Sentences.Count);
            if (randomSentenceNumber % 2 != 0)
            {
                randomSentenceNumber--;
            }
            Debug.Log("Sentences:\n" + string.Join("\n", Sentences));

            previousText += "\nScore is: " + stringPoints +
                    "\n\nPlease rearrange the following sentence:\n"
                    + Sentences[randomSentenceNumber];
        }
        return previousText;
    }

    // REFILL STRING LIST WITH SENTENCES AFTER THEY HAVE ALL BEEN USED.
    private void ResetSentences()
    {
        Sentences.Clear();
        Sentences = new List<string>(new string[] {"This crazy sentence is good", "This sentence is crazy good",
        "I went to go lake water swimming in the park", "I went to the park to go swimming in lake water",
        "She visited her grandmother on Sunday", "On Sunday she visited her grandmother",
        "Buffalo buffalo buffalo Buffalo buffalo buffalo", "Buffalo buffalo buffalo Buffalo buffalo buffalo", });
    }
}



