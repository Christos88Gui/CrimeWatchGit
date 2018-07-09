using System.Linq;
using System.Web.Mvc;
using CrimeWatch.Services;
using CrimeWatch.Models;

namespace CrimeWatch.Controllers
{
    /// <summary>
    /// Handles user requests related to counties' clustering.
    /// </summary>
    [Authorize]
    public class ClusteringController : Controller
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();
        private readonly ClusteringService _service = new ClusteringService();

        /// <summary>
        /// Redirects to counties' clustering view.
        /// </summary>
        /// <returns>Countie's clustering View with counties objects as parameter.</returns>
        public ActionResult Clusters() {
            var counties = _db.Counties.ToList();
            //Excludes City of London and Rutland counties due to the disproportionate ammount of crimes compared to their population.
            counties.Remove(counties.First(x => x.Name == "City of London"));
            counties.Remove(counties.First(x => x.Name == "Rutland"));
            return View(counties);
        }
    }
}