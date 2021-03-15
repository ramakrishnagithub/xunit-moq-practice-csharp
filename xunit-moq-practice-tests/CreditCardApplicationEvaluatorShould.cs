using System;
using Xunit;
using Moq;
using RK.Practice.Examples.Xunit.Lib;

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
    }
}
