using System;
using System.Windows.Forms;

namespace loaddatfsh
{
    /// <summary>
    /// Delays the SelectedIndexChanged event to suppress false deselection events.
    /// </summary>
    /// <remarks>
    /// As current item in the ListView is deselected before selecting the new item.
    /// This class uses a timer to ensure that the only deselection event we receive is from the user.
    /// </remarks>
    internal sealed class DelayIndexChangedListView : ListView
    {
        private Timer indexChangedTimer;

        public DelayIndexChangedListView()
        {
            this.indexChangedTimer = new Timer();
            this.indexChangedTimer.Interval = 10;
            this.indexChangedTimer.Tick += eventDelayTimer_Tick;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            this.indexChangedTimer.Start();
        }

        private void eventDelayTimer_Tick(object sender, EventArgs e)
        {
            this.indexChangedTimer.Stop();
            base.OnSelectedIndexChanged(EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.indexChangedTimer != null)
                {
                    this.indexChangedTimer.Dispose();
                    this.indexChangedTimer = null;
                }
            }
            
            base.Dispose(disposing);
        }


    }
}
