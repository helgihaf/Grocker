using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marson.Grocker.WinForms
{
    // Code in this class is based on
    // http://stackoverflow.com/questions/937298/restoring-window-size-position-with-multiple-monitors
    

    public class WindowStateManager
    {
        private readonly Form form;
        private readonly IWindowState windowStateStore;
        private bool initialized;

        public WindowStateManager(Form form, IWindowState windowStateStore)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }
            this.form = form;

            if (windowStateStore == null)
            {
                throw new ArgumentNullException(nameof(windowStateStore));
            }
            this.windowStateStore = windowStateStore;

            AddEventHandlers();
        }

        public void RestoreWindowState()
        {
            // this is the default
            form.WindowState = FormWindowState.Normal;
            form.StartPosition = FormStartPosition.WindowsDefaultBounds;

            // check if the saved bounds are nonzero and visible on any screen
            if (windowStateStore.WindowPosition != Rectangle.Empty &&
                IsVisibleOnAnyScreen(windowStateStore.WindowPosition))
            {
                // first set the bounds
                form.StartPosition = FormStartPosition.Manual;
                form.DesktopBounds = windowStateStore.WindowPosition;

                // afterwards set the window state to the saved value (which could be Maximized)
                form.WindowState = windowStateStore.WindowState;
            }
            else
            {
                // this resets the upper left corner of the window to windows standards
                form.StartPosition = FormStartPosition.WindowsDefaultLocation;

                // we can still apply the saved size
                // msorens: added gatekeeper, otherwise first time appears as just a title bar!
                if (windowStateStore.WindowPosition != Rectangle.Empty)
                {
                    form.Size = windowStateStore.WindowPosition.Size;
                }
            }
            initialized = true;
        }

        private bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddEventHandlers()
        {
            form.FormClosed += Form_FormClosed;
            form.FormClosing += Form_FormClosing;
            form.Resize += Form_Resize;
            form.Move += Form_Move;
        }

        private void RemoveEventHandlers()
        {
            form.Move -= Form_Move;
            form.Resize -= Form_Resize;
            form.FormClosing -= Form_FormClosing;
            form.FormClosed -= Form_FormClosed;
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            windowStateStore.WindowState = GetRestorableWindowState();
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveEventHandlers();
        }

        private void Form_Move(object sender, EventArgs e)
        {
            TrackWindowState();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            TrackWindowState();
        }

        public FormWindowState GetRestorableWindowState()
        {
            if (form.WindowState == FormWindowState.Maximized)
            {
                return form.WindowState;
            }
            else
            {
                return FormWindowState.Normal;
            }
        }

        public void TrackWindowState()
        {
            // Don't record the window setup, otherwise we lose the persistent values!
            if (!initialized)
            {
                return;
            }

            if (form.WindowState == FormWindowState.Normal)
            {
                windowStateStore.WindowPosition = form.DesktopBounds;
            }
        }
    }

    public interface IWindowState
    {
        Rectangle WindowPosition { get; set; }
        FormWindowState WindowState { get; set; }
    }
}
