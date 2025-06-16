using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Interface
{
    /// <summary>
    /// 定義一個物件具備「可種植」能力的合約
    /// </summary>
    public interface IPlanter
    {
        // 目前這個介面作為一個標記，告訴工廠它的類型。
        // 未來可以新增如 public PlantingSpeed { get; } 等屬性。
    }
}
