exe:
	make -C Pixie dll
	make -C Pixie.Terminal dll

all:
	make -C Pixie all
	make -C Pixie.Terminal all

dll:
	make -C Pixie dll
	make -C Pixie.Terminal dll

flo:
	make -C Pixie all
	make -C Pixie.Terminal flo

nuget:
	nuget restore Pixie.sln

clean: clean-ecsc
	make -C Pixie clean
	make -C Pixie.Terminal clean

include flame-make-scripts/use-ecsc.mk
