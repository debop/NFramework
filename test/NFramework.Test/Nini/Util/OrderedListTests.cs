#region Copyright

//
// Nini Configuration Project.
// Copyright (C) 2006 Brent R. Matzelle.  All rights reserved.
//
// This software is published under the terms of the MIT X11 license, a copy of 
// which has been included with this distribution in the LICENSE.txt file.
// 

#endregion

using NUnit.Framework;

namespace NSoft.NFramework.Nini.Util {
    [TestFixture]
    public class OrderedListTests {
        [Test]
        public void BasicOrder() {
            var list = new OrderedList
                       {
                           { "One", 1 },
                           { "Two", 2 },
                           { "Three", 3 }
                       };

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            Assert.AreEqual(1, list["One"]);
            Assert.AreEqual(2, list["Two"]);
            Assert.AreEqual(3, list["Three"]);
        }
    }
}