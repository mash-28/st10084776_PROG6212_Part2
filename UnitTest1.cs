using Microsoft.VisualStudio.TestTools.UnitTesting;
using CMCS;
using System.ComponentModel;

namespace CMCS.Tesst
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestClaimCreation()
        {
            Claim claim = new Claim
            {
                ClaimId = 1,
                Lecturer = "John Doe",
                Course = "Web Development",
                Hours = 20,
                Rate = 50,
                Status = ClaimStatus.Pending
            };

            Assert.AreEqual(1, claim.ClaimId);
            Assert.AreEqual("John Doe", claim.Lecturer);
            Assert.AreEqual("Web Development", claim.Course);
            Assert.AreEqual(20, claim.Hours);
            Assert.AreEqual(50, claim.Rate);
            Assert.AreEqual(1000, claim.Total);
            Assert.AreEqual(ClaimStatus.Pending, claim.Status);
        }

        [TestMethod]
        public void TestClaimImplementsINotifyPropertyChanged()
        {
            Claim claim = new Claim();
            Assert.IsTrue(claim is INotifyPropertyChanged, "Claim should implement INotifyPropertyChanged");
        }
    }
}