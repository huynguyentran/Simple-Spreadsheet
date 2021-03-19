A window form that emulates a spreadsheet editor. 
Author: Willian Erignac & Huy Nguyen 

<Spreadsheet 101>
- The spreadsheet was designed with the user's convenience in mind. Because of this, there are many ways to help the user edits the content of the cell box. 
- To enter the content into the cell box, the user has to highlight the cell that they want to edit. After that, the user will need to highlight the cell content box above and pass in their information. If the information is invalid, the spreadsheet will notify the user about it. To confirm the change, the user can either press the enter key, or select another cell box. 
- The user can use the spreadsheet without the mouse. After highlight a cell box, the user can use arrow keys to move around. To edit the content of the cell box, the user presses the enter key, provides the new cell contents, and presses the enter key again for confirmation. 
- The user can open, save, and close the cell through help menu. 
- Warnings will be created for any actions that may resulted errors.
- The spreadsheet also has a build in regex, the lowercase [a-z] will be transformed to their respective uppercase when the user enters into the content box. 
- Highlight depedents and disco mode were added as addtional features. 

<Addtional features> 
- The spreadsheet provides 2 additional features for the users, mainly on the dependencies of the cells. 
	<Highlight dependencies>
	-This feature will highlight the cell and other cells which are depended on it. For example, if we have A1 = 10, and B2 = A1+10, using highlight dependencies on A1 will also color B2 as well. Note that if the user highlights B2 and there are no cell depended on it, the feature will only color B2. 
	-Each dependencies will have different color. And the color will be randomize everytime. The color for different dependencies will be different from each other. 
	<Disco mode> 
	-This feature will turn the spreadsheet into a disco ball. The cells will constantly change their color between intervals. While disco mode is enabled, the user will not be able to edit any content. 
	-The spreadsheet also will detect any changes the user makes when opens and edits an existing spreadsheet.

<Design decisions> 
- To make the experience more fluid for the user. 
- Threads and backgroundworkers were ultilized to make sure the user fluid experience. 
- Modifications for spreadSheetPanel class was used to make the highlighting more easy. 
- Creates a queue when entering contents into cells. By doing this, the backgroundworker can have time to finish parsing in the current content before starting the next one. 

<External code and resources> 
-ProcessCmdKey external code was used to override the arrow key. This helps the user moves between cells using arrow keys instead of using the scroll bar of spreadsheetPanel.
-MessageBoxIcon was used to make the message box giving more information through visual images. 

<Implementation notes>


<Problems>
(03/11) The form will through when entering cells that has 11 onward (Fixed by fixing the regex)
(03/15) When open multiple forms, closing the orignal forms will cause all forms to close (Fixed by modifying open)
(03/16) The application will creates window sounds when pressing Enter (Fixed by add in e.Handled())
(03/16) When retrieving the content of the cell in the DoWork method, the DoWork method was getting the box's string after it was set to whatever contents is in the cell being moved to. (Fixed by adding a boolean so that the content of the cell is forced to be retrieved before calling DoWork)

(03/17) Threads and stuff

(03/17) When giving the errors to the user, the errors may be too cryptic for user to understand. 
(03/18) If the user tries to exit the program while disco mode is enabled, it will through an error because the BackgroundWorker may not haven't finished running. (Fixed by replacing the backgroundworker with thread, Abort method is called when the user tries to exit the program.)
(03/18) Some color may be retained in some cell if the user tries to highlight cells during disco mode (Heisen bug, maybe putting the thread to sleep for 100 second ?)
(03/18) Disco mode may take focus of other panel. (Fixed by set a condition aroudn spreadSheetPanel.Focus() in display selection.)

<Not yet>
maybe disable the spreadsheet panel entirely when disco mode is on 
