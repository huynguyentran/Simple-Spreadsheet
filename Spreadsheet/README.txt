A window form that emulates a spreadsheet editor. 
Author: Willian Erignac & Huy Nguyen 
Date: 03/19/2021

<Spreadsheet 101>
- The spreadsheet was designed with the user's convenience in mind. Because of this, there are many ways to help the user edits the content of a cell box. 
- To enter the content into a cell box, the user selects the cell that they want to edit. After that, the user selects the cell content box above and passes in the content. If the content is invalid, the spreadsheet will notify the user. To confirm the change, the user can either press the enter key or select another cell box. 
- The user can use the spreadsheet can navigate the spreadsheet using the arrow keys. After selecting a cell box, the user can use arrow keys to move around. To edit the content of the cell box, the user presses the enter key, provides the new cell contents, and presses the enter key again for confirmation. 
- The user can open, save, and close the cell through the Menu. 
- Warnings will be created for any actions that may result in errors.
- The spreadsheet also has a built-in regex, lowercase characters will be transformed to their respective uppercase when the user tries to enter a formula with variables. 
- Highlight dependents and Disco Mode™ were added as additional features. 
-The spreadsheet also detects any changes the user makes when opening a new spreadsheet or closing the current spreadsheet.

<Addtional features and how to use them> 
- The spreadsheet provides two additional features for the users, mainly on the dependencies of the cells. 
	<Highlight Dependents>
	-This feature will highlight the selected cell and other cells which are its dependents. For example, if we have A1 = 10, and B2 = A1+10, using Highlight Dependents on A1 will also highlight B2 with the same color. Note that if the user uses the feature on B2 and there is no cell that depends on it, Highlight Dependents will only color B2. 
	-Each dependency group will have a different color.
	<Disco Mode™> 
	-This feature will turn the spreadsheet into a dance floor. The cells will constantly change their color between intervals. While disco mode is enabled, the user will not be able to edit any content. 

<Design decisions> 
- Threads and backgroundworkers are used to ensure a fluid experience. 
- Creating a queue when entering contents into cells. By doing this, the background worker can have time to finish parsing in the current content before starting the next one. 
-MessageBoxIcon was used to make the message box giving more information through visual images. 

<External code and resources> 
-ProcessCmdKey external code was used to override the arrow key. This helps the user moves between cells using arrow keys instead of using the scroll bar of the spreadsheet panel.
-Interpolate was inspired by Unity's Lerp. 
-Modifications for the spreadsheetpanel class were used to make the highlighting possible. 

<Implementation notes>
- When disco mode is enabled, the user can not make any changes to make sure that to ensure the focus stays on the spreadsheetpanel.

<Problems>
(03/11) The form will throw when entering cells that have rows divisible by ten. (Fixed by modifying the regex)
(03/15) When open multiple forms, closing the original form will cause all forms to close (Fixed by modifying Program class)
(03/16) The application will create Windows sound when pressing Enter (Fixed by add-in e.Handled())
(03/16) When retrieving the content of the cell in the DoWork method, the DoWork method was getting the box's string after it was set to whatever contents is in the cell being moved to. (Fixed by adding a boolean so that the content of the cell is forced to be retrieved before calling DoWork)
(03/17) Overlapping backgroundworker run Async call when calculating the dependencies of the cell. (Fixed by creating a queue for saving content in the cell)
(03/17) When giving the errors to the user, the errors may be too cryptic for the user to understand. (Fixed by modifying the original Formula class to give better messages.) 
(03/18) If the user tries to exit the program while Disco Mode™ is enabled, it will throw an error because the backgroundworker may not have finished running. (Fixed by replacing the backgroundworker with thread, the Abort method is called when the user tries to exit the program.)
(03/19) Cells may retain colors if the user tries to disable Disco Mode™. (Fixed by not selecting the content of selection while Disco Mode™ is on)
(03/19) Disco Mode™ may take the focus from another spreadsheet form. (Fixed by setting a condition around spreadsheetpanel.Focus() in display selection.)
(03/19) The form will throw when Clear All Highlights is called without actual highlight on the screen (Fixed by not saving content when calling Clear All Highlights.)
(03/19) Open file create a new form instead of replacing the current form (Fixed by changing the opening)