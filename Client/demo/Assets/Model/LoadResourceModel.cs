using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;

namespace Assets.Model
{
    /// <summary>
    /// 加载资源类
    /// </summary>
    public class LoadResourceModel
    {
        public GameResource.ResourceType Type = GameResource.ResourceType.Prefab;
        public string Path = String.Empty;

        public LoadResourceModel()
        {
        }

        public LoadResourceModel(GameResource.ResourceType type,string path)
        {
            this.Type = type;
            this.Path = path;
        }
    }
}
