# ZipParser
Parses and outputs zip file information by reading the central directory

Platform:  
`Windows .NET Framework 4.8`

Language:  
`C# (Developed using Visual Studio 2022)`

Usage:  
`ZipParser.exe <path_to_zip>`

Sample Output:

    folder00/       True    0       2022-05-19T10:51:38
    folder00/folder00-00/   True    0       2022-05-19T10:51:18     A nested folder
    folder00/folder00-00/test00-00-00.txt   False   4       2020-08-25T09:05:38
    folder00/folder00-00/test00-00-01.txt   False   125     2022-05-19T10:56:30
    folder00/folder00-00/test00-00-02.txt   False   4       2020-08-25T09:05:38
    folder00/test00-00.txt  False   95      2022-05-19T10:57:24
    folder00/test00-01.txt  False   0       2021-08-25T01:04:38     This file doesn't have any content
    folder01/       True    0       2022-05-19T10:51:26
    folder01/exercise.zip   False   2272    2022-05-19T11:05:08
    folder01/test01-00.txt  False   127     2022-05-19T10:53:46     This is a comment
    test00.txt      False   4       2020-08-25T09:05:38     A top level file
    test01.txt      False   4       2020-08-25T09:05:38
    test02.txt      False   4       2020-08-25T09:05:38
