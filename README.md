# LongJohnSilver
A discord bot holding various specific functions for our personal discord channel.

Nothing complicated here and probably nothing particularly well written!

At present this bot plays a simple knockout game, letting you add entries (such as a list of all the James Bond films for example!)
and then let a game be played amongst the Discord'ers where a person can add one point to an existing entry and deduct one point 
from another (everyone starts with three points). People can only go three times an hours, and can go unless two other people have
been before their last go.

Purpose of the silly game is to determine which of the items is 'best'. Protect your favourites, try and eliminate your most hated.

INSTRUCTIONS

So, in order for this to function you will need to build it in the directory of your choice and provide two extra files:

In the root of the .dll directory, you will need a file called "config", which simply contains the bottoken of your bot.
Also in the root directory you will need a Data directory, and within this you will need to make a sqllite database.
Populate it using the script create.sql attached in the project.
