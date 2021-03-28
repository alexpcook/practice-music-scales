package scales

import (
	"reflect"
	"regexp"
	"testing"
)

func getMajorScalesRegexPattern() string {
	return "^[A-G] (Flat |Sharp |)Major$"
}

func getMinorScalesRegexPattern() string {
	return "^[A-G] (Flat |Sharp |)Minor$"
}

func getScalesTestRunner(tt *testing.T, getScalesFn func() []Scale, resultRegex *regexp.Regexp) {
	scales1 := getScalesFn()
	scales2 := getScalesFn()

	if !reflect.DeepEqual(scales1, scales2) {
		tt.Fatalf("expected scales1 == scales2, got %v and %v", scales1, scales2)
	}

	for _, scales := range [][]Scale{scales1, scales2} {
		for _, scale := range scales {
			if matched := resultRegex.MatchString(scale); !matched {
				tt.Fatalf("expected match with %q, got %q", resultRegex, scale)
			}
		}
	}
}

func TestGetMajorScales(tt *testing.T) {
	majorScalesRegex := regexp.MustCompile(getMajorScalesRegexPattern())
	getScalesTestRunner(tt, GetMajorScales, majorScalesRegex)
}

func TestGetMinorScales(tt *testing.T) {
	minorScalesRegex := regexp.MustCompile(getMinorScalesRegexPattern())
	getScalesTestRunner(tt, GetMinorScales, minorScalesRegex)
}

func getRandomScalesTestRunner(tt *testing.T, getRandScalesFn func() []Scale, resultRegex *regexp.Regexp) {
	scales1 := getRandScalesFn()
	scales2 := getRandScalesFn()

	if reflect.DeepEqual(scales1, scales2) {
		tt.Fatalf("expected scales1 != scales2, got %v and %v", scales1, scales2)
	}

	for _, scales := range [][]Scale{scales1, scales2} {
		for _, scale := range scales {
			if matched := resultRegex.MatchString(scale); !matched {
				tt.Fatalf("expected match with %q, got %q", resultRegex, scale)
			}
		}
	}
}

func TestGetRandomMajorScales(tt *testing.T) {
	majorScalesRegex := regexp.MustCompile(getMajorScalesRegexPattern())
	getRandomScalesTestRunner(tt, GetRandomMajorScales, majorScalesRegex)
}

func TestGetRandomMinorScales(tt *testing.T) {
	minorScalesRegex := regexp.MustCompile(getMinorScalesRegexPattern())
	getRandomScalesTestRunner(tt, GetRandomMinorScales, minorScalesRegex)
}

func TestGetMajorScalesMapKey(tt *testing.T) {
	expected := "major"
	if got := GetMajorScalesMapKey(); got != expected {
		tt.Fatalf("expected %q, got %q", expected, got)
	}
}

func TestGetMinorScalesMapKey(tt *testing.T) {
	expected := "minor"
	if got := GetMinorScalesMapKey(); got != expected {
		tt.Fatalf("expected %q, got %q", expected, got)
	}
}

func TestGetRandomScales(tt *testing.T) {
	scalesMap := GetRandomScales()

	for scaleType, scales := range scalesMap {
		var regexPattern string
		if scaleType == GetMajorScalesMapKey() {
			regexPattern = getMajorScalesRegexPattern()
		} else if scaleType == GetMinorScalesMapKey() {
			regexPattern = getMinorScalesRegexPattern()
		} else {
			tt.Fatalf("expected valid map key, got %q", scaleType)
		}

		regex := regexp.MustCompile(regexPattern)
		for _, scale := range scales {
			if matched := regex.MatchString(scale); !matched {
				tt.Fatalf("expected match with %q, got %q", regex, scale)
			}
		}
	}
}
