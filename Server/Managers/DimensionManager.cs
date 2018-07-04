using GrandTheftMultiplayer.Server.Elements;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roleplay.Server.Managers
{
    public class DimensionManager
    {
        public static readonly List<int> PrivateDimensions = new List<int>();
        public static readonly List<int> PublicDimensions = new List<int>();

        public static void RequestPrivateDimension(Client client)
        {
            Task.Run(() =>
            {
                int dim = -1;
                while (PrivateDimensions.Contains(dim))
                {
                    dim--;
                }
                PrivateDimensions.Add(dim);
                client.dimension = dim;
                client.sendNotification("", "Neue Private Dimension " + dim);
            });
        }

        public static void RequestPublicDimension(Client client)
        {
            Task.Run(() =>
            {
                int dim = 1;
                while (PublicDimensions.Contains(dim))
                {
                    dim++;
                }
                PublicDimensions.Add(dim);
                client.dimension = dim;
                client.sendNotification("", "Neue Public Dimension " + dim);
            });
        }

        public static void GoToNormalWorldDimension(Client client)
        {
            int dim = client.dimension;
            if (PrivateDimensions.Contains(dim))
            {
                PrivateDimensions.Remove(dim);
            }
            if (PublicDimensions.Contains(dim))
            {
                PublicDimensions.Remove(dim);
            }
            client.dimension = 0;
        }
    }
}