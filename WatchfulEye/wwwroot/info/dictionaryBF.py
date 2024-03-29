﻿import random
import time
import json

word_order = ["t", "p"]

# Currently trying words starting with this letter in word list
current_letter = word_order[0]
# Index of current word withing its letter's section
current_letter_index = 0

guesses = 0

digits = {1:0, 2:0, 3:0, 4:0, 5:0, 6:0, 7:0, 8:0}

current_guess = 0

all_guesses = 0

reps = [0,1]

status = "ongoing"

password_length_to_start = 1

password_length = password_length_to_start

password_length_to_generate = 3

words_list = json.loads('[{"p": ["password"], "t": ["test"]}]')[0]

characters = ["s", "a", "t", "c", "b", "d", "e", "f", "w", "g", "h", "i", \
                  "l", "p", "r", "m", "u", "n", "o", "j", "k", "x", "v", "y", \
                  "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "q", "z", \
                  "S", "A", "T", "C", "B", "D", "E", "F", "W", "G", "H", "I", \
                  "L", "P", "R", "M", "U", "N", "O", "J", "K", "X", "V", "Y", \
                  "Q", "Z"]

def test_password(*args, **kwargs):
  global current_guess, status, current_letter, current_letter_index, guesses, all_guesses
  target_password = str(Element('BruteForcePhrase').element.value)

  digits = {1:0, 2:0, 3:0, 4:0, 5:0, 6:0, 7:0, 8:0}

  total_time = 0.0


  status = "ongoing"

  current_letter = word_order[0]
  current_letter_index = 0

  if target_password == "":
    reps[0] += 1
    status = "stopped"
    print(status)

  this_time = time.time()
  guesses = 0
  while status == "ongoing":
        guesses+=1
        generate_dictionary_word(target_password)
        if current_guess == target_password:
            elapsed = time.time() - this_time
            status = "Cracked"
            pyscript.write("WE_Results", status + ": " + current_guess)
            pyscript.write("WE_Results",str(guesses + 1) + " guesses, " + str(elapsed) + " seconds.\n___________\n")
            all_guesses += guesses
            total_time += elapsed

def test_password2(*args, **kwargs):
  global current_guess, status, current_letter, current_letter_index, guesses, all_guesses
  target_password = str(Element('BruteForcePhrase').element.value)

  total_time = 0.0

  status = "ongoing"

  this_time = time.time()
  guesses = 0
  # Start guessing until correct
  while status == "ongoing":
        # Generate a new guess
        guesses += 1
        generate_brute_force()
        if current_guess == target_password:
          elapsed = time.time() - this_time
          status = "Cracked"
          pyscript.write("WE_Results", status + ": " + current_guess)
          pyscript.write("WE_Results",str(guesses + 1) + " guesses, " + str(elapsed) + " seconds.\n___________\n")
          all_guesses += guesses
          total_time += elapsed

def generate_brute_force():
    global target_password, current_guess, password_length, digits, status
    
    # Number of characters filled so far
    char_count = 0
    # Characters filled so far
    current_guess = ""
    # The current index in the digits array
    index_counter = password_length
    
    # Create the guess according to current digits array    
    while char_count < password_length:
        char_count += 1
        current_guess += characters[digits[char_count]]

    # Incerement the relative key in the index array for next run
    digits_modded = "no"
    while digits_modded != "yes":
        # Increment end index value if not equal to length of character array
        if digits[index_counter] < (len(characters) - 1):
            digits[index_counter] += 1
            digits_modded = "yes"

        # Otherwise move focus left if not already on first index in digits array    
        else:
            if index_counter > 1:
                # Reset value fo existing index
                digits[index_counter] = 0
                # Move index left
                index_counter -= 1
                # Increment the value for the new index
                if digits[index_counter] < (len(characters) - 1):
                    digits[index_counter] += 1
                    digits_modded = "yes"
            else:
                # All combinations tried. Increase password length
                #  and reset counters
                password_length += 1
                for pos in digits:
                    digits[pos] = 0                  
                digits_modded = "yes"
                print(status + ": " + current_guess)

def generate_dictionary_word(target_password):
  global current_guess, status, current_letter, current_letter_index, guesses, all_guesses

  if len(words_list[current_letter]) > 0:
    current_guess = words_list[current_letter][current_letter_index]

    if current_letter_index < (len(words_list[current_letter]) - 1):
      current_letter_index+=1

    else:
      if word_order.index(current_letter) < (len(word_order) - 1):
        current_letter = word_order[word_order.index(current_letter) + 1]
        current_letter_index = 0
        print(current_letter)

      else:
        status = "not_found"
        print("DONE: " + current_letter)
        all_guesses += (guesses + 1)
        guesses = 0