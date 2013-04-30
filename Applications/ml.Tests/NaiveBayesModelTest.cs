using ml.Supervised.NaiveBayes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ml.Attributes;
using System.Xml.Serialization;
using System.IO;

namespace CloudyBank.UnitTests.MachineLearning
{
    [TestClass()]
    public class NaiveBayesModelTest
    {

        [TestMethod]
        public void NaiveBayes_Main_Test()
        {
            var data = Payment.GetData();
            
            NaiveBayesModel<Payment> model = new NaiveBayesModel<Payment>();
            var predictor = model.Generate(data);
            var item = predictor.Predict(new Payment { Amount = 110, Description = "Monop try it" });

            Assert.AreEqual(item.Category, "Household");
        }

        //[TestMethod()]
        //public void NaiveBayes_Second_Test()
        //{
        //    var model = new NaiveBayesModel<Student>();
        //    var predictor = model.Generate(Student.GetData());

        //    var result = predictor.Predict(new Student { Name = "John", Age = 19, Friends = 8, GPA = 4.0, Tall = true });
        //    Assert.IsTrue(result.Nice);
        //}

        [TestMethod]
        public void NaiveBayesPredictor_Serialization_Test()
        {
            var data = Payment.GetData();

            NaiveBayesModel<Payment> model = new NaiveBayesModel<Payment>();
            var predictor = model.Generate(data);

            XmlSerializer ser = new XmlSerializer(predictor.GetType());

            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, predictor);

                stream.Position = 0;

                var newPredictor = model.Load(stream);
                var item = newPredictor.Predict(new Payment { Amount = 110, Description = "Monop try it" });
                Assert.AreEqual(item.Category, "Household");
            }

        }

    }
}
