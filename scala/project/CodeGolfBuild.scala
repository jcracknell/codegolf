import sbt._
import Keys._

object CodeGolfBuild extends Build {
  lazy val codegolf = Project(
    id       = "codegolf",
    base     = file("."),
    settings = Defaults.defaultSettings ++ Seq(
      version             := "0.1",
      scalaVersion        := "2.10.2",
      scalacOptions ++= Seq(
        "-feature",
        "-deprecation",
        "-Xlint",
        "-Ywarn-all"
      ),
      libraryDependencies <++= (scalaVersion){ sv => Seq(
        "org.scala-lang" % "scala-reflect" % sv,
        "org.scalatest" %% "scalatest" % "latest.snapshot" % "test",
        // `latest.snapshot` currently seems to resolve to maven central, which is out of date...
        "com.github.axel22" %% "scalameter" % "0.4-SNAPSHOT" % "test"
      ) },
      testFrameworks += new TestFramework("org.scalameter.ScalaMeterFramework"),
      resolvers += Resolver.sonatypeRepo("snapshots")
    )
  )
}
