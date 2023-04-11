using UnityEngine;

namespace PhotoMode.UI
{
    public class Metrics : MonoBehaviour
    {
        [SerializeField] private HistogramGraphic colorHistogram;
        [SerializeField] private HistogramGraphic luminanceHistogram;

		public Histogram Histogram
        {
            set
            {
                colorHistogram.Histogram = value;
                luminanceHistogram.Histogram = value;
            }
        }
    }
}
