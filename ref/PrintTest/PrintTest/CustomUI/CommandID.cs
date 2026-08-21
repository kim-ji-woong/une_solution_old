using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    internal class CommandID
    {
        public const int FILE_BEGIN = 0;
        public const int FILE_SENARIO_NEW = 1;

        public const int FILE_SENARIO_OPEN = 2;
        public const int FILE_SENARIO_SAVE = 3;

        public const int FILE_USER_VAR_OPEN = 4;
        public const int FILE_USER_VAR_SAVE = 5;

        public const int FILE_ENUM_OPEN = 6;
        public const int FILE_ENUM_SAVE = 7;

        public const int FILE_EXIT = 8;
        public const int FILE_SYSTEM_VAR_OPEN = 9;
        public const int FILE_SENARIO_SAVEAS = 10;
        public const int FILE_END = 11;

        public const int VIEW_BEGIN = 20;
        public const int VIEW_EXPR = 21;
        public const int VIEW_TEXT = 22;
        public const int VIEW_OPTION = 23;
        public const int VIEW_LEFTPANE = 24;
        public const int VIEW_EXPR_HELP = 25;
        public const int VIEW_END = 26;


        public const int EDIT_BEGIN = 40;
        public const int EDIT_COPY = 41;
        public const int EDIT_PASTE = 42;
        public const int EDIT_DELETE = 43;
        public const int EDIT_CUT = 44;
        public const int EDIT_UNDO = 45;
        public const int EDIT_REDO = 46;
        public const int EDIT_END = 47;



		public const int SENARIO_BEGIN = 60;
		public const int SENARIO_SIMULATION = 61;
		public const int SENARIO_VERIFY = 62;
		public const int SENARIO_END = 63;
    }
}
