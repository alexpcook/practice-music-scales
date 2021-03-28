package scales

import (
	"math/rand"
	"time"
)

// Scale is a type representation of a musical scale as a string
type Scale = string

// Major scale constants
const (
	CMaj  Scale = "C Major"
	GMaj  Scale = "G Major"
	DMaj  Scale = "D Major"
	AMaj  Scale = "A Major"
	EMaj  Scale = "E Major"
	BMaj  Scale = "B Major"
	FsMaj Scale = "F Sharp Major"
	CsMaj Scale = "C Sharp Major"
	FMaj  Scale = "F Major"
	BbMaj Scale = "B Flat Major"
	EbMaj Scale = "E Flat Major"
	AbMaj Scale = "A Flat Major"
	DbMaj Scale = "D Flat Major"
	GbMaj Scale = "G Flat Major"
	CbMaj Scale = "C Flat Major"
)

// Minor scale constants
const (
	AMin  Scale = "A Minor"
	EMin  Scale = "E Minor"
	BMin  Scale = "B Minor"
	FsMin Scale = "F Sharp Minor"
	CsMin Scale = "C Sharp Minor"
	GsMin Scale = "G Sharp Minor"
	DsMin Scale = "D Sharp Minor"
	AsMin Scale = "A Sharp Minor"
	DMin  Scale = "D Minor"
	GMin  Scale = "G Minor"
	CMin  Scale = "C Minor"
	FMin  Scale = "F Minor"
	BbMin Scale = "B Flat Minor"
	EbMin Scale = "E Flat Minor"
	AbMin Scale = "A Flat Minor"
)

// Seed the PRNG upon package import
func init() {
	rand.Seed(time.Now().UnixNano())
}

// GetMajorScales returns the major scales in order (no flats/sharps,
// increasing number of sharps, increasing number of flats).
func GetMajorScales() []Scale {
	return []Scale{
		CMaj, GMaj, DMaj, AMaj, EMaj, BMaj, FsMaj, CsMaj,
		FMaj, BbMaj, EbMaj, AbMaj, DbMaj, GbMaj, CbMaj,
	}
}

// GetMinorScales returns the minor scales in order (no flats/sharps,
// increasing number of sharps, increasing number of flats).
func GetMinorScales() []Scale {
	return []Scale{
		AMin, EMin, BMin, FsMin, CsMin, GsMin, DsMin, AsMin,
		DMin, GMin, CMin, FMin, BbMin, EbMin, AbMin,
	}
}

// GetRandomMajorScales returns the major scales in random order
func GetRandomMajorScales() []Scale {
	scales := GetMajorScales()
	rand.Shuffle(len(scales), func(i, j int) {
		scales[i], scales[j] = scales[j], scales[i]
	})
	return scales
}

// GetRandomMinorScales returns the minor scales in random order
func GetRandomMinorScales() []Scale {
	scales := GetMinorScales()
	rand.Shuffle(len(scales), func(i, j int) {
		scales[i], scales[j] = scales[j], scales[i]
	})
	return scales
}

// GetMajorScalesMapKey returns the key used for major scales in maps
func GetMajorScalesMapKey() string {
	return "major"
}

// GetMinorScalesMapKey returns the key used for minor scales in maps
func GetMinorScalesMapKey() string {
	return "minor"
}

// GetRandomScales returns the major and minor scales in random order
func GetRandomScales() map[string][]Scale {
	return map[string][]Scale{
		GetMajorScalesMapKey(): GetRandomMajorScales(),
		GetMinorScalesMapKey(): GetRandomMinorScales(),
	}
}
