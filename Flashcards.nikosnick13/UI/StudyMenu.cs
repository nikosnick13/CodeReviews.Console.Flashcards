using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using static Flashcards.nikosnick13.Enums.Enums;
using Flashcards.nikosnick13.Controllers;


namespace Flashcards.nikosnick13.UI;

internal class StudyMenu
{


    private readonly StudyController _studyController;
    private readonly StackController _stackController;

    public StudyMenu()
    {
        _studyController = new StudyController();
        _stackController = new StackController();

    }

    public void ShowStudyMenu() 
    {

        bool isStudyRunning = true;

        while (isStudyRunning)
        {
            Clear();
            var studyMenu = AnsiConsole.Prompt(
                new SelectionPrompt<StudyMenuOptions>()
                .Title("What would you like to do?")
                .AddChoices(
                    StudyMenuOptions.StartStudySession,
                    StudyMenuOptions.ViewStudySessions,
                    StudyMenuOptions.ReturnToMainMenu
                    ));

            switch (studyMenu) 
            {
                case StudyMenuOptions.StartStudySession:
                    StartStudySession();            
                    break;
                case StudyMenuOptions.ViewStudySessions:
                    _studyController.ViewStudySessions();
                    break;
                case StudyMenuOptions.ReturnToMainMenu:
                    isStudyRunning = false;
                    break;
            }
        }
    }

    private void StartStudySession() {


        Clear();
        AnsiConsole.MarkupLine("[green]Starting a new study session...[/]\n");
        ReadKey();

        _stackController.ViewAllStacks();
        int stackId = AnsiConsole.Ask<int>("Enter the [blue] Stack Id [/] you want to stady:");

         _studyController.StartStudySession(stackId);
       

    }


}
