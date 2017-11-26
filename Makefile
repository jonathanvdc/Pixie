exe:
	make -C Pixie dll
	make -C Pixie.Terminal dll
	make -C Examples exe

all:
	make -C Pixie all
	make -C Pixie.Terminal all
	make -C Examples all

dll:
	make -C Pixie dll
	make -C Pixie.Terminal dll

flo:
	make -C Pixie all
	make -C Pixie.Terminal all
	make -C Examples flo

nuget:
	nuget restore Pixie.sln

clean: clean-ecsc
	make -C Pixie clean
	make -C Pixie.Terminal clean
	make -C Examples clean

test: exe
	mono ./Examples/CaretDiagnostics/bin/clr/CaretDiagnostics.exe
	mono ./Examples/FormattedList/bin/clr/FormattedList.exe
	mono ./Examples/ParseOptions/bin/clr/ParseOptions.exe

include flame-make-scripts/use-ecsc.mk
