using GameLogic.Interface;
using GameLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Manager
{
    public static class CropFactory
    {
        /// <summary>
        /// 根據傳入的作物類型，動態建立一個新的作物實例。
        /// </summary>
        /// <param name="cropType">要建立的作物的 System.Type</param>
        /// <returns>一個新的 Crop 物件</returns>
        public static Crop CreateCrop(IPlanter planter)
        {
            return planter switch
            {
                GenericFarmer => new GeneralCrop(),
                WheatFarmer => new Wheat(),
                RiceFarmer => new Rice(),
                _ => throw new NotSupportedException($"無法為類型 {planter.GetType().Name} 建立作物。"),
            };
        }
    }
}
