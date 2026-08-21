using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpaceText : MonoBehaviour
{
    public Mark MoveXMark;
    public Mark MoveYMark;
    public Mark MoveBothMark;
    
    public void Start()
    {
        MoveXMark.Active(false);
        MoveYMark.Active(false);
        MoveBothMark.Active(false);
    }

    public void Select(CustomizingStage stage)
    {
        if (stage == CustomizingStage.Mode_Move)
        {
            //MoveXMark.Active(true);
            //MoveYMark.Active(true);
            MoveBothMark.Active(true);
        }
        else
        {
            //MoveXMark.Active(false);
            //MoveYMark.Active(false);
            MoveBothMark.Active(false);
        }
    }
}
