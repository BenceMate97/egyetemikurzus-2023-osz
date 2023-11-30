﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameController
{
    private FileManager<string> answerFileManager;
    private FileManager<Player> playerFileManager;
    private List<string> words;
    private List<Player> players;

    public GameController()
    {
        answerFileManager = new FileManager<string>("Answers.json");
        playerFileManager = new FileManager<Player>("Player.json");
        words = answerFileManager.LoadData();
        players = playerFileManager.LoadData();
    }


    public string getInput()
    {
        string input = Console.ReadLine();
        if(input == null)
        {
            input = "";
        }
        return input;
    }

    public void startGameController()
    {
        string input = "";
        loadMenu(false);
        input = getInput();   //elso input a jatekostol

        Console.WriteLine(input);
        while (input != "6")
        {
            if (input == "1" || input == "2" || input == "3" || input == "4" || input == "5")
            {
                loadMenu(false);
                switch (input)
                {
                    case "1":
                        newGame();
                        input = "";
                        break;
                    case "2":
                        showRules();
                        input = "";
                        break;
                    case "3":
                        showHighScore();
                        input = "";
                        break;
                    case "4":
                        showAnswerList();
                        input = "";
                        break;
                    case "5":
                        //showAddNewWord();
                        input = "";
                        break;
                }
            }
            else if (input == "")
            {
                loadMenu(false);
                input = getInput();
            }
            else
            {
                loadMenu(true);
                input = getInput();
            }
        }
        loadMenu(false);
        Console.WriteLine("Thanks for checking out my game!\n" +
                          "Press any key, to close the program");

        Console.ReadKey();
        return;
    }

    public void newGame()
    {

        logo();
        string currentAnswer = GetAnAnswer();
        Console.WriteLine(currentAnswer);
        bool lost = false;
        int collectedScore = 0, reachedRound = 0;
        while (!lost)
        {
            logo();
            Console.WriteLine(currentAnswer);
            Console.WriteLine("Round " + reachedRound + ", score: " + collectedScore);
            Guess[] guessFeedbacks = new Guess[6];
            string[] allMyGuesses = new string[6];
            for (int i = 0; i < 6; i++) //6-ot tippelhetunk
            {
                tries(i+1);
                string currentGuess = getInput();
                
                while (!isInValidGuess(currentGuess))
                {                    
                    printPreviousLines(guessFeedbacks, allMyGuesses, i+1,reachedRound,collectedScore);
                    currentGuess = getInput();
                }
                allMyGuesses[i] = currentGuess;
                if (currentGuess.ToUpper() == currentAnswer.ToUpper())
                {
                    switch (i)
                    {
                        case 0:
                            collectedScore += 100;
                            break;
                        case 1:
                            collectedScore += 80;
                            break;
                        case 2:
                            collectedScore += 60;
                            break;
                        case 3:
                            collectedScore += 40;
                            break;
                        case 4:
                            collectedScore += 20;
                            break;
                        case 5:
                            collectedScore += 5;
                            break;
                    }
                    reachedRound++;
                    currentAnswer = GetAnAnswer();
                    break;
                }
                else
                {
                    guessFeedbacks[i] = new Guess(currentAnswer, currentGuess);
                    Console.WriteLine("\t\t    "+ guessFeedbacks[i].getFeedback());
                    if (i == 5)
                    {
                        lost = true;
                    }
                }
            }

        }
        saveGame(collectedScore, reachedRound, currentAnswer);
        Console.ReadKey();

    }

    public bool isInValidGuess(string guess)
    {
        if (guess == null)
        {
            return false;
        }
        if (guess.Length != 5)
        {
            return false;
        }        
        return guess.All(char.IsLetter);
    }

    public void printPreviousLines(Guess[] guesses, string[] feedbacks,int piece,int round,int score)
    {
        logo();
        Console.WriteLine("Wrong input, the guess should be a 5 letter long word!");
        Console.WriteLine("Round " + round + ", score: " + score);
        for (int i = 0; i < piece; i++)
        {
            if (i != piece - 1)
            {
                tries(i + 1);
                Console.WriteLine(feedbacks[i]);
            }
            else
            {
                tries(i + 1);
                //Console.Write(guesses[i].getFeedback());
            }
            if (i != piece - 1)
            {
                Console.WriteLine("\t\t    " + guesses[i].getFeedback());
            }
        }
    }

    public void saveGame(int score, int round, string answer)
    {
        logo();
        Console.WriteLine("You couldn't guess the word: " + answer + "\nYou got " + score + "points in " + round + " rounds!" +
                            "\nGive us your name, to save your scores.");
        string name = "";
        name = getInput();
        while (name == "")
        {
            Console.WriteLine("You need to give us a name.");
            name = getInput();
        }
        Player currentplayer = new Player
        {
            name = name.ToUpper(),
            score = score,
            round = round,
        };
        players.Add(currentplayer);
        playerFileManager.SaveData(players);
        logo();
        Console.WriteLine("Your record is now saved " + name);
    }

    public void showCurrentGame(int currentround, int currentscore, string[] previousGuessesAndFeedback)
    {
        logo();
    }

    public void tries(int currentTry)
    {
        switch (currentTry)
        {
            case 1:
                Console.Write("1st guess (100pts): ");
                break;
            case 2:
                Console.Write("2nd guess  (80pts): ");
                break;
            case 3:
                Console.Write("3rd guess  (60pts): ");
                break;
            case 4:
                Console.Write("4th guess  (40pts): ");
                break;
            case 5:
                Console.Write("5th guess  (20pts): ");
                break;
            case 6:
                Console.Write("6th guess   (5pts): ");
                break;
        }
    }

    public void showRules()
    {
        logo();
        Console.Write("Objective:\n\tThe goal of Wordle is to guess a hidden 5 letter long Word within 6 guesses\n\n" +
                      "Feedback:\n\tPlayer make guesses by suggesting words that they think might be the hidden word.\n" +
                      "\tEach guess has to be 5 letter long.\n" +
                      "\tAfter the guess, the player get a line of feedback for each letter in the guesseg word\n" +
                      "\tExample: the hidden word is \"PLANE\", and the player guessed \"LEAPT\"\n" +
                      "\t\t|LEAPT|\n" +
                      "\t\t|▄▄█▄X|\n\n" +
                      "\tThe symbols meaning:\n" +
                      "\t\t▄ - the letter is in the word, but not in there (in the example \"L\" is in the 1st position, while in the hidden word it's the 2nd letter\n" +
                      "\t\t█ - the letter is in the word, AND in the right place(\"A\" is the 3rd letter in PLANE and in LEAPT)\n" +
                      "\t\tX - the letter is not part of the hidden word, try to guess a word that don't have this letter\n" +
                      "Points:\n" +
                      "\tThe player will have 6 guesses for each new word:\n" +
                      "\tPoints are taken only when you guessed the word correctly.\n" +
                      "\t\tif you guess the word the 1st time, you gain 100pts (sometimes you get lucky).\n" +
                      "\t\tfor each guess you get 20pts less.\n" +
                      "\t\t\t1.try: 100pts\n" +
                      "\t\t\t2.try: 80pts\n" +
                      "\t\t\t3.try: 60pts\n" +
                      "\t\t\t4.try: 40pts\n" +
                      "\t\t\t5.try: 20pts\n" +
                      "\t\t\t6.try: 5pts\n\n" +
                      "If a word you tried cant be a hidden word, you get a warning, and it wont count as a try, so you won't lose a try for it\n" +
                      "You can check the possible words in the \"Possible Words\" option in the menu\n" +
                      "If you can't guess the word in 6 tries, your points will be stored in the highscores, after you give a player name\n" +
                      "\n" +
                      "Press any key to get back to menu");
        Console.ReadKey();

    } //done

    public void showHighScore()
    {
        logo();
        var top10Players = players
            .GroupBy(player => player.name)
            .Select(group => group.OrderByDescending(player => player.score).First())
            .OrderByDescending(player => player.score)
            .ThenBy(player => player.name)
            .Take(10)
            .ToList();
        Console.WriteLine("Top 10 (or less) Players by reached score:");
        int place = 1;
        foreach (var player in top10Players)
        {
            Console.WriteLine($"{place}. {player.name} - with {player.score} points in {player.round} round.");
            place++;
        }

        Console.WriteLine("\nYou can check for any players records, if you give us it's username (or write \"exit\") to get back to menu:");
        string input = getInput().ToUpper();
        while(input != "EXIT")
        {
            searchPlayer(players, input);
            input = getInput().ToUpper();
        }
        Console.WriteLine("You are getting back to the menu. Press any key to continue!");
        Console.ReadKey();
    } //done

    public void searchPlayer(List<Player> playerRecords, string playerName)
    {
        var playerDetails = playerRecords
            .Where(player => player.name == playerName)
            .ToList();

        if (playerDetails.Any())
        {
            int highestScore = playerDetails.Max(player => player.score);
            double averageScore = playerDetails.Average(player => player.score);
            int highestRound = playerDetails.Max(player => player.round);
            double averageRound = playerDetails.Average(player => player.round);

            logo();
            Console.WriteLine($"{playerName} details:");
            Console.WriteLine($"   Highest score: {highestScore}  Average score: {averageScore:F2}");
            Console.WriteLine($"   Highest round: {highestRound}  Average round: {averageRound:F2}");
            Console.WriteLine("\nYou can check for any players records, if you give us it's username (or write \"exit\" to get back to menu:");
        }
        else
        {
            logo();
            Console.WriteLine($"Player with {playerName} name does not played yet.");
            Console.WriteLine("\nYou can check for any players records, if you give us it's username (or write \"exit\" to get back to menu:");
        }
    } //done

    public void showAnswerList()
    {
        logo();
        foreach (string word in words)
        {
            Console.WriteLine(word);
        }

        Console.WriteLine($"\nCurrent possible answers: {words.Count}\nPress any key to get back to menu");
        Console.ReadKey();

    } //done

    public void showAddNewWord()
    {

    } //TODO

    public void logo()
    {
        Console.Clear();
        Console.WriteLine(" __      __________ __________________  .____     ___________\r\n/  \\    /  \\_____  \\\\______   \\______ \\ |    |    \\_   _____/\r\n\\   \\/\\/   //   |   \\|       _/|    |  \\|    |     |    __)_ \r\n \\        //    |    \\    |   \\|    `   \\    |___  |        \\\r\n  \\__/\\  / \\_______  /____|_  /_______  /_______ \\/_______  /\r\n       \\/          \\/       \\/        \\/        \\/        \\/ \n");
    } //done

    public void loadMenu(bool isInputWrong)
    {
        logo();
        if (isInputWrong)
        {
            Console.WriteLine("Press a number to navigate!\n!! A number from the list below !!\n");
        }
        else
        {
            Console.WriteLine("Press a number to navigate!\n\n");
        }
        Console.WriteLine("1. New Game\n" +
                          "2. Rules\n" +
                          "3. High-Score\n" +
                          "4. Possible words\n" +
                          "5. Add word\n" +
                          "6. Exit Game\n");
    }   //done

    public void AddNewWord(string newWord)
    {
        string upperCaseWord = newWord.ToUpper();
        if (!words.Contains(upperCaseWord))
        {
            if (upperCaseWord.Length == 5)
            {
                words.Add(upperCaseWord.ToUpper());
                answerFileManager.SaveData(words);
            }
            else
            {
                Console.WriteLine("The word must be 5 characters long.");
            }
        }
        else
        {
            Console.WriteLine("This word is already on the list");
        }
    }

    public string GetAnAnswer()
    {
        if (words == null)
        {
            words = answerFileManager.LoadData();
        }
        if (words.Count > 0)
        {
            Random random = new Random();
            int index = random.Next(words.Count);
            return words[index];
        }
        else
        {
            return "No words available.";
        }
    }
    
    public void getWords()
    {
        foreach (string word in words)
        {
            Console.WriteLine(word);
        }
    }
}
