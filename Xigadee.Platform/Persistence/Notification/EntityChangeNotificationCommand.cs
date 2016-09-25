using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This command transmits command change notifications, and can also listen for changes and notify subscribing services.
    /// </summary>
    public class EntityChangeNotificationCommand:CommandBase<EntityChangeNotificationStatistics, EntityChangeNotificationPolicy>
    {
        public EntityChangeNotificationCommand()
        {

        }

        public void Publish(EntityChangeEventArgs change)
        {

        }
    }
}
