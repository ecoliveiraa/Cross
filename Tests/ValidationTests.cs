using NUnit.Framework;
using WebApplication1.Models;
using WebApplication1.Validation;
using WebApplication1.Configuration;

namespace WebApplication1.Tests
{
    [TestFixture]
    public class ValidationTests
    {
        private IRuleManager _ruleManager;

        [SetUp]
        public void Setup()
        {
            _ruleManager = new RuleManager();
            RuleConfiguration.ConfigureRules(_ruleManager);
        }

        [Test]
        public void ValidateCompany_PortugalWithNIF_ShouldPass()
        {
            // Arrange
            var company = new Company
            {
                Country = "Portugal",
                NIF = "123456789",
                Address = "Lisboa",
                Stakeholder = "João",
                Contact = "joao@test.pt"
            };

            // Act
            var result = _ruleManager.Validate(company);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void ValidateCompany_PortugalWithoutNIF_ShouldFail()
        {
            // Arrange
            var company = new Company
            {
                Country = "Portugal",
                NIF = "", // ← Vazio!
                Address = "Lisboa",
                Stakeholder = "João",
                Contact = "joao@test.pt"
            };

            // Act
            var result = _ruleManager.Validate(company);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.That(result.Errors[0], Does.Contain("NIF is required"));
        }

        [Test]
        public void ValidateCompany_SpainWithoutNIF_ShouldPass()
        {
            // Arrange
            var company = new Company
            {
                Country = "Spain",
                NIF = "", // ← Vazio mas OK porque não é Portugal
                Address = "Madrid",
                Stakeholder = "Carlos",
                Contact = "carlos@test.es"
            };

            // Act
            var result = _ruleManager.Validate(company);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void ValidateLead_ActiveWithoutBusinessType_ShouldFail()
        {
            // Arrange
            var lead = new Lead
            {
                CompanyId = Guid.NewGuid(),
                BusinessType = "", // ← Vazio!
                Status = "Active"
            };

            // Act
            var result = _ruleManager.Validate(lead);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.That(result.Errors[0], Does.Contain("BusinessType is required"));
        }

        [Test]
        public void ValidateLead_DraftWithoutBusinessType_ShouldPass()
        {
            // Arrange
            var lead = new Lead
            {
                CompanyId = Guid.NewGuid(),
                BusinessType = "", // ← Vazio mas OK porque é Draft
                Status = "Draft"
            };

            // Act
            var result = _ruleManager.Validate(lead);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void ValidateProposal_FinalizedWithoutMonthlyProducts_ShouldFail()
        {
            // Arrange
            var proposal = new Proposal
            {
                LeadID = Guid.NewGuid(),
                ProductionCost = 10000,
                MonthlyProducedProducts = null, // ← Null!
                Status = "Finalized"
            };

            // Act
            var result = _ruleManager.Validate(proposal);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors.Any(e => e.Contains("MonthlyProducedProducts")));
        }
    }
}