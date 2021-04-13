using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
	public class SendWeeklyEmail : Signals.Core.Processes.Recurring.RecurringProcess<VoidResult>
	{
        public override RecurrencePatternConfiguration Profile => 
            new WeeklyRecurrencePatternConfiguration(1) { RunNow = true }
            .On(System.DayOfWeek.Tuesday)
            .At(8, 0, 0);

        /// <summary>
        /// Send weekly email to your users
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override VoidResult Sync()
        {
            // send emails
            return Ok();
        }
    }
}