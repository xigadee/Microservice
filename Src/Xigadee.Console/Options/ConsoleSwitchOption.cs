using System;

namespace Xigadee
{
    /// <summary>
    /// This is class that holds the options entry on the console menu
    /// </summary>
    public class ConsoleSwitchOption: ConsoleOption
    {

        #region Constructors
        public ConsoleSwitchOption(
              string textSwitchedOff, Func<ConsoleMenu, ConsoleSwitchOption, bool> switchOn
            , string textSwitchedOn, Func<ConsoleMenu, ConsoleSwitchOption, bool> switchOff
            , ConsoleMenu childMenu = null
            , Func<ConsoleMenu, ConsoleSwitchOption, bool> switched = null
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , string shortcut = null
            )
            : base(textSwitchedOff, null, childMenu, enabled, display, null, shortcut)
        {
            Action = (m,o) =>
            {
                ConsoleSwitchOption to = o as ConsoleSwitchOption;

                if (IsSwitchedOn)
                    IsSwitchedOn = !switchOff(m,to);
                else
                    IsSwitchedOn = switchOn(m,to);

                if (switched != null)
                {
                    if (switched(m, to))
                    {
                    }
                }

                Text = IsSwitchedOn? textSwitchedOn : textSwitchedOff;
            };
        }
        #endregion

        /// <summary>
        /// This boolean switch identifies whether the option has been enabled.
        /// </summary>
        public bool IsSwitchedOn { get; set; }


    }
}
