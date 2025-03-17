using Dapper;
using Flashcards.nikosnick13.Models;
using Flashcards.nikosnick13.UI;
using Microsoft.Data.SqlClient;
using Spectre.Console;
using System.Configuration;
using static System.Console;

namespace Flashcards.nikosnick13.Controllers;

internal class StudyController
{
    private string? connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

    public void StartStudySession(int stackId)
    {
        try {

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Id,Question, Answer FROM Flashcards WHERE Stack_Id = @stackId";
            var flashcards = conn.Query<Flashcard>(query, new { stackId }).ToList();

            if (flashcards.Count == 0) {

                AnsiConsole.MarkupLine($"[red]❌ No flashcards found for Stack ID [/]{stackId}.");
                ReadKey();
                return;
            }

            int correctAnswers = 0;
            int totalQuestions = flashcards.Count;

            foreach (var card in flashcards) {

                Clear();
                WriteLine($"Qustion:[blue]{card.Question}[/]");

                string userAnswer = AnsiConsole.Ask<string>(" Your answer:");

                if (userAnswer.Trim().Equals(card.Answer.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    correctAnswers++;
                    AnsiConsole.MarkupLine("[green] Correct!!![/]");
                }
                else { 

                    AnsiConsole.MarkupLine($"[red]Wrong![/] The correct answer was: [yellow]{card.Answer}[/]");
                }
                AnsiConsole.Prompt(new TextPrompt<string>("\nPress [green]Enter[/] to continue...").AllowEmpty());
            }

            int score = (int)((double)correctAnswers / totalQuestions * 100); // Ποσοστό επιτυχίας

            InsertStudySession(stackId, score);

        } catch(Exception ex) {
            WriteLine($"Error starting study session: {ex.Message}");
        }
    }

    public void InsertStudySession(int stackId, int score)
    {
        try { 
       
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string query = @"INSERT INTO StudySessions(Stack_Id,Date,Score)VALUES(@stackId,@date,@score) ";

            conn.Execute(query, new
            {
                stackId = stackId,
                date = DateTime.Now,
                score = score
            });

            WriteLine($"\n✅ Study session for Stack ID {stackId} added with score {score}%!");
            ReadKey();
        }
        catch(Exception ex){
            WriteLine($"❌ Error: {ex.Message}");
        }
    }

    public void ViewStudySessions() {
        try {

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string query = @"SELECT Id, Stack_Id AS StackId, Date, Score FROM StudySessions ORDER BY Date DESC ";

            var studySessions = conn.Query<StudySession>(query).ToList();

            TableVisualisation.DisplayStudySession(studySessions);
        }
        catch (Exception ex) {

            WriteLine($"Error retrieving study sessions: {ex.Message}");
        }
    }

    
}