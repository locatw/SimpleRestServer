using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestServer.Tests
{
    public class HttpStatusTest
    {
        [TestClass]
        public class EqualityTest
        {
            [TestMethod]
            public void EqualsMethodReturnsTrueWhenSameStatusCodeAndDescription()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(200, "OK");

                Assert.IsTrue(status1.Equals(status2));
            }

            [TestMethod]
            public void EqualsMethodReturnsFalseWhenSameStatusCodeButDifferentDescription()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(200, "Success");

                Assert.IsFalse(status1.Equals(status2));
            }

            [TestMethod]
            public void EqualsMethodReturnsFalseWhenSameDescriptionButDifferentStatusCode()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(201, "OK");

                Assert.IsFalse(status1.Equals(status2));
            }

            [TestMethod]
            public void OperatorEqualReturnsTrueWhenSameStatusCodeAndDescription()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(200, "OK");

                Assert.IsTrue(status1 == status2);
            }

            [TestMethod]
            public void OperatorEqualReturnsFalseWhenSameStatusCodeButDifferentDescription()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(200, "Success");

                Assert.IsFalse(status1 == status2);
            }

            [TestMethod]
            public void OperatorEqualReturnsFalseWhenSameDescriptionButDifferentStatusCode()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(201, "OK");

                Assert.IsFalse(status1 == status2);
            }

            [TestMethod]
            public void OperatorNotEqualReturnsFalseWhenSameStatusCodeAndDescription()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(200, "OK");

                Assert.IsFalse(status1 != status2);
            }

            [TestMethod]
            public void OperatorNotEqualReturnsTrueWhenSameStatusCodeButDifferentDescription()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(200, "Success");

                Assert.IsTrue(status1 != status2);
            }

            [TestMethod]
            public void OperatorNotEqualReturnsTrueWhenSameDescriptionButDifferentStatusCode()
            {
                var status1 = new HttpStatus(200, "OK");
                var status2 = new HttpStatus(201, "OK");

                Assert.IsTrue(status1 != status2);
            }
        }
    }
}
