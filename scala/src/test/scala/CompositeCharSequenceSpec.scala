import org.scalatest.{Inside, FunSpec, Matchers}

class CompositeCharSequenceSpec extends FunSpec with Matchers with Inside {
  /** Produce a string representation of the provided [[java.lang.CharSequence]], possibly a
    * [[smd.util.CompositeCharSequence]], as `toString` an a [[java.lang.CharSequence]] must return
    * the equivalent string representation. */
  def treeString(cs: CharSequence): String = cs match {
    case ccs: CompositeCharSequence =>
      s"${ccs.getClass.getSimpleName}(${treeString(ccs.left)},${treeString(ccs.right)})"
    case _ => LiteralEncoding.encode(cs)
  }

  describe("charAt") {
    it("should behave as expected") {
      val cs = CompositeCharSequence("a","b")
      cs.charAt(0) should be ('a')
      cs.charAt(1) should be ('b')
      intercept[IndexOutOfBoundsException] { cs.charAt(2) }
    }
  }

  describe("equals") {
    it("should consider an equivalent CharSequence equal") {
      CompositeCharSequence("one", "two") should equal ("onetwo")
    }
    it("should consider an empty string equal") {
      CompositeCharSequence("", "") should equal ("")
    }
  }

  describe("balanced construction") {
    it("should work for 2 parts") {
      val cs = CompositeCharSequence.balanced(Seq("one", "two"))
      cs.length should be (6)
      cs.toString should be ("onetwo")

      inside(cs) { case CompositeCharSequence(l, r) =>
        l should be ("one")
        r should be ("two")
      }
    }
    it("should work for 3 parts") {
      val cs = CompositeCharSequence.balanced(Seq("one", "two", "three"))
      cs.length should be (11)
      cs.toString should be ("onetwothree")
    }
    it("should work for 4 parts") {
      val cs = CompositeCharSequence.balanced(Seq("one", "two", "three", "four"))
      cs.length should be (15)
      cs.toString should be ("onetwothreefour")

      inside(cs) { case CompositeCharSequence(l, r) =>
        inside(l) { case CompositeCharSequence(l, r) =>
          l should be ("one")
          r should be ("two")
        }
        inside(r) { case CompositeCharSequence(l, r) =>
          l should be ("three")
          r should be ("four")
        }
      }
    }
    it("should work for 5 parts") {
      val cs = CompositeCharSequence.balanced(Seq("one","two","three","four","five"))
      cs.length should be (19)
      cs.toString should be ("onetwothreefourfive")
    }
    it("should work for 6 parts") {
      val cs = CompositeCharSequence.balanced(Seq("one", "two", "three", "four", "five", "six"))
      cs.length should be (22)
      cs.toString should be ("onetwothreefourfivesix")
    }
  }
  describe("weighted construction") {
    it("should work for two parts")  {
      val cs = CompositeCharSequence.weighted(Seq("one", "two"), balancedBelow = 0)
      cs.length should be (6)
      cs.toString should be ("onetwo")
    }
    it("should work for 3 parts") {
      val cs = CompositeCharSequence.weighted(Seq("one","two","three"), balancedBelow = 0)
      cs.length should be (11)
      cs.toString should be ("onetwothree")
    }
    it("should work for 4 parts")  {
      val cs = CompositeCharSequence.weighted(Seq("one","two","three","four"), balancedBelow = 0)
      cs.length should be (15)
      cs.toString should be ("onetwothreefour")
    }
    it("should work for 5 parts") {
      val cs = CompositeCharSequence.weighted(Seq("one","two","three","four","five"), balancedBelow = 0)
      cs.length should be (19)
      cs.toString should be ("onetwothreefourfive")
    }
    it("should work for 6 parts") {
      val cs = CompositeCharSequence.weighted(Seq("one","two","three","four","five","six"), balancedBelow = 0)
      cs.length should be (22)
      cs.toString should be ("onetwothreefourfivesix")
    }
    it("should construct the expected tree 1") {
      val left = CompositeCharSequence.weighted(Seq("abcdefgh", "a", "b", "c", "d", "e", "f", "g", "h"), balancedBelow = 0)
      left.toString should be ("abcdefghabcdefgh")

      inside(left) { case CompositeCharSequence(left, right) =>
        left should be ("abcdefgh")
      }

      val right = CompositeCharSequence.weighted(Seq("a", "b", "c", "d", "e", "f", "g", "h", "abcdefgh"), balancedBelow = 0)
      right.toString should be ("abcdefghabcdefgh")

      inside(right) { case CompositeCharSequence(left, right) =>
        right should be ("abcdefgh")
      }
    }
  }
}
