files = \
	*.cs

libraries = \
	../Pixie/bin/clr/Pixie.dll \
	../Pixie.Terminal/bin/clr/Pixie.Terminal.dll \
	../Pixie.Loyc/bin/clr/Pixie.Loyc.dll \
	../packages/NUnit.3.10.1/lib/net45/nunit.framework.dll \
	../packages/NUnitLite.3.10.1/lib/net45/nunitlite.dll \
	../packages/Loyc.Essentials.26.2.0/lib/net45/Loyc.Essentials.dll \
	../packages/Loyc.Collections.26.2.0/lib/net45/Loyc.Collections.dll \
	../packages/Loyc.Syntax.26.2.0/lib/net45/Loyc.Syntax.dll

ifeq ($(ASM_VERSION),)
	ASM_VERSION:=0.0.1.0
endif

common_options = -asm-name=Tests -asm-version=$(ASM_VERSION) -fno-whole-program -Wall -Wextra

.PHONY: all flo exe clean
all: flo exe

flo: bin/flo/Tests.flo
exe: bin/clr/Tests.exe

clean:
	rm -rf bin/
	rm -rf obj/

include ../flame-make-scripts/use-ecsc.mk

bin/flo/Tests.flo: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform ir -runtime clr -libs $(libraries) -o $@ $(common_options)

bin/clr/Tests.exe: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform clr -libs $(libraries) -o $@ $(common_options)
