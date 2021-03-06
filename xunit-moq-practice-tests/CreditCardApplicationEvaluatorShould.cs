using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using RK.Practice.Examples.Xunit.Lib;
using System.Linq;

namespace RK.Practice.Examples.Xunit.Tests
{
    public partial class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptHighIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application =new CreditCardApplication{ GrossAnnualIncome = 100_000 };
            CreditCardApplicationDecision decision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }        

        [Fact]
        public void ReferYoungApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 19 };            
            
            CreditCardApplicationDecision decision = sut.Evaluate(application);
           
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        /// <summary>
        ///  Return value mock method and argument matching
        /// </summary>
        [Fact]
        public void DeclineLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid("x")).Returns(true); //Return value Setup
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(
            //                  x => x.IsValid(It.Is<string>(number => number.StartsWith("y"))))
            //             .Returns(true);
            //mockValidator.Setup(
            //              x => x.IsValid(It.IsInRange("a", "z", Moq.Range.Inclusive)))
            //             .Returns(true);
            //mockValidator.Setup(
            //              x => x.IsValid(It.IsIn("z", "y", "x")))
            //             .Returns(true);
            mockValidator.Setup(
                          x => x.IsValid(It.IsRegex("[a-z]")))
                         .Returns(true);
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        /// <summary>
        ///  Mock Strict behaviour Fact implementation  
        /// </summary>
        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            //Implemented strict mock. Throws exception if mock setup is not implemented. 
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);

            /*Comment below line to verify the behaviour of strict. Error will be (
                invocation failed with mock behavior Strict. All invocations on the mock must 
                have a corresponding setup.)*/            
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication();
            CreditCardApplicationDecision decision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        /// <summary>
        ///  Out Parameter Fact implementation
        /// </summary>
        [Fact]        
        public void DeclineLowIncomeApplicationsOutDemo()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            bool isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        /// <summary>
        /// //Property Return value Fact implementation
        /// </summary>
        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.LicenseKey).Returns("EXPIRED"); //Property Return value Fact implementation

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 42 };
            CreditCardApplicationDecision decision = sut.Evaluate(application);
            
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        /// <summary>
        /// Return value from function
        /// </summary>
        [Fact]
        public void ReferWhenLicenseKeyExpired_ReturnValueFromFunction()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.LicenseKey).Returns(GetLicenseKeyExpiryString);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        /// <summary>
        /// Auto Mock hierarchies
        /// </summary>
        [Fact]
        public void ReferWhenLicenseKeyExpired_AutoMockHierarchies()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.EvaluateUsingServiceInformation(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void ReferYoungApplicationsWithAutoMock()
        {
           var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            
            /* Without the below property the test fails with Object reference error bcz ServiceInformation 
                is not setup.To Fix the problem used DefaultValue Mock
            */
            mockValidator.DefaultValue = DefaultValue.Mock; //Without this pro
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 19 };
            //uncomment for with Automock hierarchy
            CreditCardApplicationDecision decision = sut.EvaluateUsingServiceInformation(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenseKeyExpiryString()
        {
            // E.g. read from vendor-supplied constants file
            return "EXPIRED";
        }

        /// <summary>
        /// Track Property changes.Mock generally does not remember properties so
        /// need to use SetupProperty or SetupAllProperties.
        /// </summary>
        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.SetupProperty(x => x.ValidationMode);
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
        
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 30 };
            sut.EvaluateUsingServiceInformation(application);

            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }

        /// <summary>
        /// Verifying method was called or not
        /// </summary>
        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "q"
            };

            sut.EvaluateUsingServiceInformation(application);

            //verifying method is called or not
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()));

            //verifying method is called or not and throw exception 
            //if IsValid is disabled in CreditCardApplication Evaluate method.
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), "Frequent flyer number should be validated.");
        }

        /// <summary>
        ///  Verifying method was not called Test case
        /// </summary>
        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.EvaluateUsingServiceInformation(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        ///  Verify Property Getter is invoked test.
        /// </summary>
        [Fact]
        public void CheckLicenseKeyForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

            sut.EvaluateUsingServiceInformation(application);

            mockValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        /// <summary>
        ///  Verify
        /// </summary>
        [Fact]
        public void SetDetailedLookupForOlderApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 30 };

            sut.EvaluateUsingServiceInformation(application);

            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);           

            // mockValidator.VerifyNoOtherCalls();
        }

        /// <summary>
        ///  Raise exception test case 
        /// </summary>
        [Fact]
        public void ReferWhenFrequentFlyerValidationError()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator
                = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                         .Throws(new Exception("Custom message"));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.EvaluateUsingServiceInformation(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        /// <summary>
        /// Raise event test case.
        /// </summary>

        [Fact]
        public void IncrementLookupCount()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                         .Returns(true)
                        .Raises(x => x.ValidatorLookupPerformed += null, EventArgs.Empty); //Comment this line to fail test as event raise does not occur.

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { FrequentFlyerNumber = "x", Age = 25 };

            sut.EvaluateUsingServiceInformation(application);

            //mockValidator.Raise(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);

            Assert.Equal(1, sut.ValidatorLookupCount);
        }

        /// <summary>
        /// Return different values for multiple calls test case.
        /// </summary>
        [Fact]
        public void ReferInvalidFrequentFlyerApplications_ReturnValuesSequence()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
                         .Returns(false)
                         .Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision firstDecision = sut.EvaluateUsingServiceInformation(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, firstDecision);

            CreditCardApplicationDecision secondDecision = sut.EvaluateUsingServiceInformation(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, secondDecision);
        }

        /// <summary>
        ///  Multiple times method call test case.
        /// </summary>
        [Fact]
        public void ReferInvalidFrequentFlyerApplications_MultipleCallsSequence()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var frequentFlyerNumbersPassed = new List<string>();
            mockValidator.Setup(x => x.IsValid(Capture.In(frequentFlyerNumbersPassed)));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application1 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "aa" };
            var application2 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "bb" };
            var application3 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "cc" };

            sut.EvaluateUsingServiceInformation(application1);
            sut.EvaluateUsingServiceInformation(application2);
            sut.EvaluateUsingServiceInformation(application3);

            // Assert that IsValid was called three times with "aa", "bb", and "cc"
            Assert.Equal(new List<string> { "aa", "bb", "cc" }, frequentFlyerNumbersPassed);
        }
    }
}
