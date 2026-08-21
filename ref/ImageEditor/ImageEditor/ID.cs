using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    class ID
    {
        //메뉴 - 파일탭
        public const int FILE_NEW = 100;
        public const int FILE_OPEN = 101;
        public const int FILE_SAVE = 102;
        public const int FILE_SAVES = 103;
        public const int FILE_EXIT = 104;

        //메뉴 - 보기탭
        public const int VIEW_EXPANSION = 200;
        public const int VIEW_REDUCTION = 201;
        //격자
        public const int VIEW_GRID = 202;
        public const int VIEW_RULER = 203;

        //리본버튼
        public const int EDIT_COPY = 10;
        public const int EDIT_CUT = 11;
        public const int EDIT_PASTE = 12;
        public const int EDIT_DELETE = 13;
        public const int EDIT_SIZESETUP = 14;
        public const int EDIT_ROTATE = 15;
        public const int EDIT_ALLSELECT = 16;
        public const int EDIT_REVERSE = 17;
        public const int EDIT_TRANSPARENT = 18;
        public const int EDIT_SELECTCUT = 19;

        //툴바 메뉴
        public const int TOOLBAR_SELECT_AREA = 300;
        public const int TOOLBAR_SELECT_COLOR = 301;
        public const int TOOLBAR_LINE_COLOR = 302;
        public const int TOOLBAR_ZOOMIN = 303;
        public const int TOOLBAR_ZOOMOUT = 304;
        public const int TOOLBAR_TRANSLATE = 305;
        public const int TOOLBAR_ROTATE = 306;
        public const int TOOLBAR_STRAIGHT_LINE = 307;
        public const int TOOLBAR_CURVE = 308;
        public const int TOOLBAR_TEXT = 309;

        public const int TOOLBAR_STRONG = 310;
        public const int TOOLBAR_LEAN = 311;
        public const int TOOLBAR_UNDERLINE = 312;
    }
}
