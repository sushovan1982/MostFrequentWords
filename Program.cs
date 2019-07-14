using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace FrequentWords
{
    //Trie class for storing words as tree structure
    class Trie
    {
        public char c;
        Trie[] children;
        public int wordCount;
        public bool isWord;

        public Trie()
        {
            c = '\0';
            children = new Trie[26];
            wordCount = 0;
            isWord = false;
        }

        //Adding each character to Trie 
        //Limiting the scope to lower case characters only
        public int addCharacters(string input)
        {
            if (input.Length == 0)
            {
                isWord = true;
                wordCount++;
                return wordCount;
            }
            else
            {
                if (Convert.ToInt16(Convert.ToChar(input.Substring(0, 1))) < 97 || Convert.ToInt16(Convert.ToChar(input.Substring(0, 1))) > 122)
                {
                    return 0;
                }
                int pos = Convert.ToInt16(Convert.ToChar(input.Substring(0, 1))) - 97;
                if (children[pos] == null)
                {
                    children[pos] = new Trie();
                    children[pos].c = Convert.ToChar(input.Substring(0, 1));
                }
                return children[pos].addCharacters(input.Substring(1));
            }
        }

        //Checking if the word already exists, if yes, returning the updated count
        public int checkWord(string input)
        {
            if (input.Length == 0)
            {
                if (isWord)
                {
                    wordCount++;
                }
                return wordCount;
            }

            if (Convert.ToInt16(Convert.ToChar(input.Substring(0, 1))) < 97 || Convert.ToInt16(Convert.ToChar(input.Substring(0, 1))) > 122)
            {
                return 0;
            }
            int pos = Convert.ToInt16(Convert.ToChar(input.Substring(0, 1))) - 97;
            if (children[pos] == null)
            {
                return wordCount;
            }

            return children[pos].checkWord(input.Substring(1));
        }
    }

    //Data structure to be used in heap
    class DynamicArray
    {
        public string word;
        public int wordCount;

        public DynamicArray()
        {
            word = null;
            wordCount = 0;
        }
    }

    //Heap for calculating, sorting and storing max occurance of words
    class MaxHeap
    {
        public DynamicArray[] dArray = new DynamicArray[10];//Array for storing 10 most frequent words

        //Adding new word to array and sorting it
        public void addToDynamicArray(string input, int count)
        {
            if (dArray[9] == null)
            {
                for (int i = 0; i < dArray.Length; i++)
                {
                    if (dArray[i] == null)
                    {
                        dArray[i] = new DynamicArray();
                        dArray[i].word = input;
                        dArray[i].wordCount = count;
                        for (int j = i; j >= 1; j--)
                        {
                            if (dArray[j - 1].wordCount < dArray[j].wordCount)
                            {
                                string temp = dArray[j - 1].word;
                                int temp1 = dArray[j - 1].wordCount;

                                dArray[j - 1].word = dArray[j].word;
                                dArray[j - 1].wordCount = dArray[j].wordCount;
                                dArray[j].word = temp;
                                dArray[j].wordCount = temp1;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                if (dArray[9].wordCount < count)
                {
                    for (int i = dArray.Length - 1; i >= 0; i--)
                    {
                        if (i == 0 && dArray[i].wordCount < count)
                        {
                            dArray[i].word = input;
                            dArray[i].wordCount = count;
                            break;
                        }
                        else if (i == 0 && dArray[i].wordCount >= count)
                        {
                            dArray[i + 1].word = input;
                            dArray[i + 1].wordCount = count;
                            break;
                        }
                        else if (dArray[i].wordCount < count)
                        {
                            dArray[i].word = dArray[i - 1].word;
                            dArray[i].wordCount = dArray[i - 1].wordCount;
                        }
                    }
                }
            }
        }

        //Updating array with count for existing word
        public bool updateDynamicArray(string input, int count)
        {
            bool isUpdated = false;
            if (dArray[9] != null)
            {
                if (dArray[9].wordCount < count)
                {
                    for (int j = 9; j >= 1; j--)
                    {
                        if (dArray[j].word == input)
                        {
                            isUpdated = true;
                            dArray[j].wordCount = count;
                            if (dArray[j - 1].wordCount < dArray[j].wordCount)
                            {
                                string temp = dArray[j - 1].word;
                                int temp1 = dArray[j - 1].wordCount;

                                dArray[j - 1].word = dArray[j].word;
                                dArray[j - 1].wordCount = dArray[j].wordCount;
                                dArray[j].word = temp;
                                dArray[j].wordCount = temp1;
                            }
                        }
                    }
                }
            }
            return isUpdated;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Reading the input file
            string inputText = System.IO.File.ReadAllText(@"..\cantrbry\alice29.txt");
            inputText = Regex.Replace(inputText, @"(\s+|@|&|'|\(|\)|<|>|#)", " ");//Replacing special characters with space

            string temp = null;
            
            Trie newTrie = new Trie();
            MaxHeap mHeap = new MaxHeap();

            //Passing each word to Trie and Heap functions
            while (inputText.Length > 0 && inputText.LastIndexOf(" ") > 0)
            {
                temp = inputText.Substring(0, inputText.IndexOf(" "));
                if (temp.Length > 1 && !temp.Any(c => char.IsDigit(c)))
                {
                    int checkWord = newTrie.checkWord(temp.ToLower());
                    if (checkWord == 0) //If the word does not exist, add it to Heap
                    {
                        int wordCount = newTrie.addCharacters(temp.ToLower());
                        mHeap.addToDynamicArray(temp.ToLower(), wordCount);
                    }
                    else //If the word already exists, update the count in Heap
                    {
                        mHeap.updateDynamicArray(temp.ToLower(), checkWord);
                    }
                }
                inputText = inputText.Substring(inputText.IndexOf(" ") + 1);
            }

            //Print the sorted words by frequency from Heap
            for (int i = 0; i < mHeap.dArray.Length; i++)
            {
                if (mHeap.dArray[i] != null)
                {
                    Console.WriteLine(mHeap.dArray[i].word.ToString() + " -> " + mHeap.dArray[i].wordCount.ToString());
                }
            }

            Console.Read();
        }
    }
}
