using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Siemens
{
    public enum CommandType
    {
        MakeDirectory,
        ChangeDirectory,
        ChangeToRoot,
        MakeFile,
        DisplayAllDirItems,
        DisplayeAllDirAndSubDirItems,
        Exit,
        Help
    }
    public class FileSimulatorCommand
    {
        public CommandType CommandType;
        public string Parameter;
    }
    public class FileSimulatorDriver
    {
        private FileSystemManager fileSystemManager;
        private bool continueToRun = true;

        public FileSimulatorDriver()
        {
            fileSystemManager = new FileSystemManager();
            fileSystemManager.ExitEventHandler += new FileSystemManager.ExitHandler(Exit);
        }

        private void Exit()
        {
            continueToRun = false;
        }

        internal void Start()
        {
            PrintHelpMenu();
            while (continueToRun)
            {
                //PrintHelpMenu();
                ProcessUserInput(GetUserInput());
                Console.WriteLine("*********************************************");
            }
            
        }

        private void ProcessUserInput(FileSimulatorCommand command)
        {
            fileSystemManager.ProcessCommand(command);
        }

        private FileSimulatorCommand GetUserInput()
        {
            string input = Console.ReadLine();
            if (input.StartsWith("md ")) return new FileSimulatorCommand() { CommandType = CommandType.MakeDirectory, Parameter = input.Substring(3)};
            if (input.StartsWith("cd ")) return new FileSimulatorCommand() { CommandType = CommandType.ChangeDirectory, Parameter = input.Substring(3) };
            if (input.StartsWith("cd..")) return new FileSimulatorCommand() { CommandType = CommandType.ChangeToRoot };
            if (input.StartsWith("mf ")) return new FileSimulatorCommand() { CommandType = CommandType.MakeFile, Parameter = input.Substring(3) };
            if (input.StartsWith("dir /s")) return new FileSimulatorCommand() { CommandType = CommandType.DisplayeAllDirAndSubDirItems };
            if (input.StartsWith("dir")) return new FileSimulatorCommand() { CommandType = CommandType.DisplayAllDirItems };
            if (input.StartsWith("exit")) return new FileSimulatorCommand() { CommandType = CommandType.Exit};
            if (input.StartsWith("help")) return new FileSimulatorCommand() { CommandType = CommandType.Help };
            //throw custom exception that command not recongnized
            throw new CommandNotFoundException("Command not found. Please try \"help\" to see a list of valid commands");
        }

        private void PrintHelpMenu()
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
    }
}
