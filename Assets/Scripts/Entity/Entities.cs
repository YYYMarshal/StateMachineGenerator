/********************************************************************
	created:	2020/11/12
	created:	12:11:2020   9:03
	filename: 	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\Entity\Entities.cs
	file path:	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\Entity
	file base:	Entities
	file ext:	cs
	author:		YYYXB
	
	purpose:	持有全部的State和Transition
*********************************************************************/
using System;
using System.Collections.Generic;

public class Entities
{
    #region 单例
    private static readonly Lazy<Entities> lazy = new Lazy<Entities>(() => new Entities());
    public static Entities Instance
    {
        get
        {
            return lazy.Value;
        }
    }
    #endregion

    public List<StateEntity> ListState = new List<StateEntity>();
    public List<TransitionEntity> ListTransition = new List<TransitionEntity>();
}
