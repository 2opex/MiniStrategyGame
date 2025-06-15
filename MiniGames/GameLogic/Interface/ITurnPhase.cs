using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Interface
{
    /// <summary>
    /// 定義一個遊戲回合中單一階段的介面
    /// </summary>
    internal interface ITurnPhase
    {
        /// <summary>
        /// 執行此階段的遊戲邏輯
        /// </summary>
        /// <param name="context">當前的遊戲狀態</param>
        void Execute(GameContext context);
    }
}
