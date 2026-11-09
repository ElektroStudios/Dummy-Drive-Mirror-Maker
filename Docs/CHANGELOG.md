# DDMM (Dummy Drive Mirror Maker) Change Log 📋

## v1.4 *(current)* 🆕
#### 🛠️ Fixes:
 - Fixed a bug where a destination directory whose name begins with the name of the source directory (e.g. 'C:\Data2' for 'C:\Data') was wrongly rejected as a subfolder of the source directory.
 - The comparison of source and destination directories is now case-insensitive, as Windows paths are.
 - Fixed a bug where user directories at the root of a drive whose name starts like a system folder (e.g. 'D:\Recycled Photos', 'D:\RECYCLER Backup') were silently ignored.
 - The contents of an ignored directory (hidden directory, mount point or symbolic link) are no longer mirrored as regular directories and files.
 - When the symbolic links option is enabled, a mirrored directory symbolic link now keeps its own name (previously it took the name of its target), and a mirrored file symbolic link now points to the original target instead of to the source link.
 - The files located in the top level of a source drive (e.g. 'D:\') are now mirrored even when the hidden files option is disabled (the root of a drive is marked as hidden by Windows). The hidden/system attributes and timestamps of a drive root are no longer applied to the destination directory.
 - Fixed a crash at the end of a mirror operation when the source contains more than 100 directories and no files were processed (e.g. only empty directories, or the operation was aborted before processing any file).
 - Fixed a possible freeze of the application when pressing 'ABORT' at the very end of a mirror operation.
 - Fixed network (UNC) paths, which were converted to invalid long paths.
 - Fixed typos in some messages.
 - Fixed the window freezing (and moving jerkily) during a mirror operation: the progress was reported to the UI for every processed file, flooding the UI thread. The UI is now refreshed at a fixed rate.
 - The 'Overwrite destination files' option now behaves as its name says: when enabled, the existing destination files are replaced with dummy files without asking (and no questions are made about existing destination directories); when disabled, the existing destination files are kept untouched, including their timestamps and attributes (previously they were modified), and no file conflict questions are made either.
 - Fixed the overwriting of destination files that were marked as read-only, hidden or system by a previous mirror operation (with the 'Preserve file attributes' option enabled).
 - Building the mirror again into the same destination no longer fails for the symbolic links that were already mirrored: an identical link is now left as is.
 - The long path prefix ('\\?\') is now used only for paths that require it, so it no longer appears in messages for regular paths.
 - Fixed the progress bar not reaching the end of the bar when some directories were ignored (e.g. hidden directories with the hidden files option disabled), while its text said '100%'. Every directory now counts as processed, and the percentage shown is the real one.
 - The ignored (not copied) files are now counted and reported too, not only the ignored directories.
 - Fixed directories wrongly reported as ignored when mirroring a mirror (or any source that contains directory junctions or symbolic links): the contents of link targets were listed as if they were part of the source. The directory listing no longer enters links, system directories, or hidden directories when the hidden files option is disabled.
 - The target of protected directory junctions (e.g. the legacy 'My Music' junction inside 'Documents') can now be read, so they can be mirrored.
 - Fixed the icon of the status bar disappearing when a very long path was shown: long paths are now shortened with an ellipsis (the full path is shown in the tooltip).
 - Fixed the mirror operation failing with "The process cannot access the file because it is being used by another process" when the destination directory (or any of its subdirectories) was open in Windows Explorer. The timestamps are now applied with a sharing mode compatible with other processes.
 - Fixed the timestamps of the destination directories with the 'Preserve date stamps' option enabled: they were modified when their files and subdirectories were created afterwards. They are now applied when the mirror operation ends.
 - The number of directories no longer counts the source directory itself, nor the directories excluded by the options (hidden directories, directory junctions and symbolic links that are not mirrored) or by design (system directories and volume mount points).
 - Fixed the progress bar keeping the value and color of the previous mirror operation until the directories were listed.
#### 🌟 Improvements:
 - Redesigned user interface: modern layout with section headers instead of group boxes, options in three columns, the GitHub button and theme switch below the banner, and a regular-sized 'Build Mirror' / 'Abort' button next to the progress bar (replaces the huge button).
 - The window title now shows the application version.
 - New option: 'Mirror directory junctions'. Directory junctions are mirrored as new junctions that point to the same original target (volume mount points are never mirrored).
 - New option: 'Mirror hard links'. All the names (hard links) of the same source file are mirrored as hard links of a single dummy file.
 - Added light and dark themes, switchable with a toggle switch in the main window. The dark theme also darkens the title bar. The selected theme is saved in the user settings; on first run, the application follows the Windows apps theme.
 - Added a button to open the GitHub repository of the application in the default web browser.
 - The progress bar now shows the result of the mirror operation with its color: green when everything was mirrored, yellow when some items could not be mirrored (or the operation was aborted), and red when some items failed. The items excluded by the options chosen by the user (hidden items, directory junctions, symbolic links, existing destination files with the overwrite option disabled) are not warnings: they are only listed in the report. The text of the progress bar is drawn with an outline in the dark theme, so it is readable over any bar color.
 - Added a report of the mirror operation, with its result, the statistics of the source directory (subdirectories, links and hidden directories), the mirrored files and directories, and the ignored (not copied) files and directories with the reason of each one (errors, warnings, and items excluded by the options or by design). It can be opened with the new 'Details...' button next to the progress bar (always available when the operation ends) or from the question shown at the end of the mirror operation (only when some items could not be mirrored), copied to the clipboard, or saved to a text file.
 - The progress bar shows a short result ('Copied with warnings', 'Copied with errors', 'Aborted' or 'Failed') that always fits inside the bar, and the numbers are shown with thousands separators (e.g. '3,285').
 - Symbolic links can now be mirrored without administrator privileges when Windows Developer Mode is enabled.
 - Added a unit test project (MSTest).
 - Internal fixes in the background worker (pause/resume, synchronous run when the work throws an exception) and the progress bar (format strings with curly braces, value changed notifications).

## v1.3 🔄
#### 🌟 Improvements:
 - The application is now long-path aware.
 - Minor changes, fixes and improvements.

## v1.2 🔄
#### 🛠️ Fixes:
 - Fixed a crash that occurs when the selected source directory is empty.
#### 🌟 Improvements:
 - The files located in top level of selected source directory are now also mirrored.

## v1.1 🔄
#### 🚀 New Features:
 - Support to ignore the recycle bin folder in earlier Windows operating systems than VISTA.
 - Support for reparse points detection (symbolic links and mount points). note: mount points are ignored/not copied.
 - Added new UI option to choose symbolic links behavior.
#### 🌟 Improvements:
 - Improved internal file and directory sorting

## v1.0 🔄
Initial Release.