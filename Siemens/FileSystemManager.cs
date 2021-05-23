using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens
{

    public enum FileType { File, Directory }
    public abstract class FileSystemItem
    {
        public abstract FileType FileType { get; }
        public abstract string Name { get; set; }
    }

    public class DirectoryItem : FileSystemItem
    {
        private const int MAX_NO_OF_FILES_IN_A_DIR = 100;
        private const int MAX_NO_OF_DIR_IN_A_DIR = 100;
        private int noOfFilesInDirectory = 0;
        private int noOfSubDirInDirectory = 0;
        //can make constructor here
        public DirectoryItem(string dirName)
        {
            Name = dirName;
            //can be made configurable
            Files = new FileItem[MAX_NO_OF_FILES_IN_A_DIR];
            Directories = new DirectoryItem[MAX_NO_OF_DIR_IN_A_DIR];
        }
        public override FileType FileType { get { return FileType.Directory; } }
        public override string Name { get; set; }
        public FileItem[] Files { get; set; }
        public DirectoryItem[] Directories { get; set; }

        public void AddFile(string fileName)
        {
            Files[noOfFilesInDirectory++] = new FileItem(fileName);
        }

        public string[] GetAllDirectories()
        {
            var directoryNames = new string[noOfSubDirInDirectory];
            int index = 0;

            foreach (var dir in Directories)
            {
                if (dir != null)
                    directoryNames[index++] = dir.Name;
            }
            return directoryNames;
        }

        public string[] GetAllFiles()
        {
            var fileNames = new string[noOfFilesInDirectory];
            int index = 0;
            foreach (var file in Files)
            {
                if (file != null)
                    fileNames[index++] = file.Name;
                //else break;
            }
            return fileNames;
        }

        public string[] GetAllItems()
        {
            //need in array?
            var files = GetAllFiles();
            var directories = GetAllDirectories();
            var result = new string[noOfFilesInDirectory + noOfSubDirInDirectory];
            int index = 0;
            foreach (var file in files)
            {
                result[index++] = file;
            }
            foreach (var dir in directories)
            {
                result[index++] = dir;
            }
            return result;
        }

        public DirectoryItem GetDirectory(string dirName)
        {
            return Directories.Where(x => x != null && dirName.Equals(x.Name)).FirstOrDefault();
        }

        public void AddDirectory(string dirName)
        {
            Directories[noOfSubDirInDirectory++] = new DirectoryItem(dirName);
        }
    }

    public class FileItem : FileSystemItem
    {
        public FileItem(string fileName)
        {
            Name = fileName;
        }
        public override FileType FileType { get { return FileType.File; } }
        public override string Name { get; set; }
    }

    public class FileSystemManager
    {
        private DirectoryItem RootFolder { get; set; }
        private DirectoryItem CurrentFolder { get; set; }

        public delegate void ExitHandler();
        public event ExitHandler ExitEventHandler;

        public FileSystemManager()
        {
            RootFolder = new DirectoryItem("Root");
            CurrentFolder = RootFolder;
        }

        public void MakeDirectory(string dirName)
        {
            var dir = CurrentFolder.GetDirectory(dirName);
            if (dir != null)
            {
                Console.WriteLine("Directory already exists. Unable to add");
                return;
            }
            CurrentFolder.AddDirectory(dirName);
        }

        public void ChangeDirectory(string dirName)
        {
            var dir = CurrentFolder.GetDirectory(dirName);
            CurrentFolder = dir;
        }

        public void ChangeDirectory()
        {
            CurrentFolder = RootFolder;
        }

        public void PrintHelp()
        {
            Console.WriteLine("md [directory name] Creates a directory, For example: md dir1\n" +
            "cd [directory name] Changes the current directory, for example: cd dir1\n" +
            "cd..Changes the current directory to parent directory\n" +
            "mf[file name] Creates a file, for example: mf file.txt[File will not have any content]\n" +
            "dir Displays list of files and subdirectories in current directory\n" +
            "dir / s Displays files in specified directory and all subdirectories\n" +
            "exit Quits the program\n" +
            "help Prints a help menu, with short description of all the commands(optional)");
        }

        internal void ProcessCommand(FileSimulatorCommand command)
        {
            switch (command.CommandType)
            {
                case CommandType.MakeDirectory:
                    MakeDirectory(command.Parameter);
                    break;
                case CommandType.ChangeDirectory:
                    ChangeDirectory(command.Parameter);
                    break;
                case CommandType.ChangeToRoot:
                    ChangeDirectory();
                    break;
                case CommandType.MakeFile:
                    CreateFile(command.Parameter);
                    break;
                case CommandType.DisplayAllDirItems:
                    DisplayDirectoriesAndFile();
                    break;
                case CommandType.DisplayeAllDirAndSubDirItems:
                    DisplayAllSubDirAndDir();
                    break;
                case CommandType.Exit:
                    ExitEventHandler();
                    break;
                case CommandType.Help:
                    PrintHelp();
                    break;
                default:
                    break;
            }
        }

        private void DisplayAllSubDirAndDir()
        {
            DisplayAllSubDirAndDirUtil(CurrentFolder);
        }

        private void DisplayAllSubDirAndDirUtil(DirectoryItem dir)
        {
            var fileItem = dir.GetAllItems();
            foreach (var itemName in fileItem)
            {
                Console.WriteLine(itemName);
            }
            foreach (var subDirName in dir.Directories)
            {
                if (subDirName != null)
                    DisplayAllSubDirAndDirUtil(subDirName);
            }
        }

        private void DisplayDirectoriesAndFile()
        {
            var fileItem = CurrentFolder.GetAllItems();
            foreach (var itemName in fileItem)
            {
                Console.WriteLine(itemName);
            }
        }

        private void CreateFile(string fileName)
        {
            CurrentFolder.AddFile(fileName);
            //Console.WriteLine("Added file " + fileName);
        }

    }


}
