exe:
	make -C Pixie dll
	make -C Pixie.Terminal dll
	make -C Pixie.Loyc dll
	make -C Examples exe
	make -C Tests exe

all:
	make -C Pixie all
	make -C Pixie.Terminal all
	make -C Pixie.Loyc all
	make -C Examples all
	make -C Tests all

dll:
	make -C Pixie dll
	make -C Pixie.Terminal dll
	make -C Pixie.Loyc dll

flo:
	make -C Pixie all
	make -C Pixie.Terminal all
	make -C Pixie.Loyc all
	make -C Examples flo
	make -C Tests flo

nuget:
	nuget restore Pixie.sln

clean: clean-ecsc
	make -C Pixie clean
	make -C Pixie.Terminal cle
	make -C Pixie.Loyc clean
	make -C Examples clean
	make -C Tests clean

run-examples: exe
	mono ./Examples/CaretDiagnostics/bin/clr/CaretDiagnostics.exe
	mono ./Examples/FormattedList/bin/clr/FormattedList.exe
	mono ./Examples/LoycInterop/bin/clr/LoycInterop.exe
	mono ./Examples/ParseOptions/bin/clr/ParseOptions.exe a.txt -fno-syntax-only --files -O1 -Ofast b.txt --files=c.txt - -- -v
	mono ./Examples/PrintHelp/bin/clr/PrintHelp.exe
	mono ./Examples/SimpleErrorMessage/bin/clr/SimpleErrorMessage.exe

test: exe
	mono ./Tests/bin/clr/Tests.exe

include flame-make-scripts/use-ecsc.mk
