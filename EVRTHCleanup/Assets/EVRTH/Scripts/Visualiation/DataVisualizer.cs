using System.Collections;
using Geometry;
using GlobeNS;
using UnityEngine;
using Utility;

namespace Visualiation
{
    public abstract class DataVisualizer : MonoBehaviour
    {
        protected AbstractGlobe globe;
        protected Transform transformToWrap;
        protected ScienceDataIngestor dataIngestor;

        public virtual void SetVisualizationSources(AbstractGlobe globe, Transform target, ScienceDataIngestor dataIngestor)
        {
            this.globe = globe;
            transformToWrap = target;
            this.dataIngestor = dataIngestor;
        }

        public abstract void GenerateVisualization();

        public virtual void Reset()
        {

        }

        public LatLongValue GetLatLonFrom3DPoint(Vector3 point)
        {
            return LatLongValue.GetLatLonFrom3DPoint(point, transformToWrap);
        }

        public virtual IEnumerator DeformBasedOnNewTile(Tile tile, float min, float max)
        {
            yield return 0;
        }
    }
}
