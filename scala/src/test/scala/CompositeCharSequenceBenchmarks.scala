import org.scalameter.{Gen, PerformanceTest}

object CompositeCharSequenceBenchmarks extends PerformanceTest.Quickbenchmark {

  performance of "CompositeCharSequence" in {
    val impls = Gen.enumeration("implementation")(string, balanced, weighted)
    val parts = Gen.exponential("part count")(from = 10, until = 100000, factor = 10)
                .map { n =>
                  val r = new scala.util.Random(42)
                  Array.fill(n) { new String(Array.fill(r.nextInt(128) + 1)('a')) }
                }
                .cached

    measure method "construction" in {
      using(Gen.tupled(impls, parts)) in { case (impl, ps) =>
        impl.create(ps)
      }
    }

    measure method "charAt" in {
      val accesses = Gen.single("accesses")(1000000)
      val cases = Gen.tupled(impls, parts, accesses)
                  .map { case (i, ps, a) => (i.create(ps), a) }
                  .cached

      using(cases) in { case (charSeq, as) =>
        val length = charSeq.length
        for(i <- 0 to as)
          charSeq.charAt(i % length)
      }
    }
  }

  //region Implementations

  abstract class Implementation {
    def create(parts: Array[String]): CharSequence
  }

  /** Baseline implementation; the alternative to using a [[CompositeCharSequence]] is to concatenate
    * all parts into a string. */
  case object string extends Implementation {
    def create(parts: Array[String]): CharSequence = {
      val sb = new StringBuilder(parts.map(_.length).sum)
      for(p <- parts)
        sb.append(p)
      sb.toString
    }
  }

  /** [[CompositeCharSequence]] using balanced construction. */
  case object balanced extends Implementation {
    def create(parts: Array[String]): CharSequence =
      CompositeCharSequence.balanced(parts)
  }

  /** [[CompositeCharSequence]] using weighted construction. */
  case object weighted extends Implementation {
    def create(parts: Array[String]): CharSequence =
      CompositeCharSequence.weighted(parts)
  }

  //endregion
}
