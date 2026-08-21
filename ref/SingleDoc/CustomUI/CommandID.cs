using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreSafe
{
    internal class CommandID
    {
        public const int FILE_BEGIN = 0;
        public const int FILE_SENARIO_OPEN = 1;
        public const int FILE_SENARIO_OSAVE = 2;
        public const int FILE_USER_VAR_OPEN = 3;
        public const int FILE_USER_VAR_SAVE = 4;

        public const int FILE_ENUM_OPEN = 5;
        public const int FILE_ENUM_SAVE = 6;
        public const int FILE_EXIT = 7;
        public const int FILE_SYSTEM_VAR_OPEN = 8;
        public const int FILE_END = 9;

        public const int VIEW_BEGIN = 10;
        public const int VIEW_EXPR = 11;
        public const int VIEW_TEXT = 12;
        public const int VIEW_OPTION = 13;
        public const int VIEW_LEFTPANE = 14;
        public const int VIEW_EXPR_HELP = 15;
        public const int VIEW_END = 16;


        public const int EDIT_BEGIN = 20;
        public const int EDIT_COPY = 21;
        public const int EDIT_PASTE = 22;
        public const int EDIT_DELETE = 23;
        public const int EDIT_CUT = 24;
        public const int EDIT_UNDO = 25;
        public const int EDIT_REDO = 26;
        public const int EDIT_END = 29;
    }
}
